using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using CrudGeneratorTest.Models.TO;

namespace CrudGeneratorTest.Models.DAL
{
    public class ItemListaDAL
    {
        public static IList<ItemListaTO> Get(int start, int pageSize, ref int totRegistros, string textoFiltro, ref int totRegistrosFiltro, string sortColumn, string sortColumnDir)
        {
            IList<ItemListaTO> objs = new List<ItemListaTO>();

            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                string ordenacao;
                if (string.IsNullOrEmpty(sortColumn))
                {
                    ordenacao = "ORDER BY Id";
                }
                else
                {
                    ordenacao = string.Format("ORDER BY {0} {1}", sortColumn, sortColumnDir);
                }
                StringBuilder queryGet = new StringBuilder(@"
                SELECT TOP (@pageSize) *
                FROM (
				    SELECT 
                    Id,
                    Nome,
                    TipoLista,
                    IdPai,

                    (ROW_NUMBER() OVER (").Append(ordenacao).Append(@"))
                    AS 'numeroLinha', 

                    (SELECT COUNT(Id) FROM ItemLista) 
				    AS 'totRegistros', 

					(SELECT COUNT(Id) FROM ItemLista 
					    WHERE
                        Id like @textoFiltro
                        OR
                        Nome collate Latin1_General_CI_AI like @textoFiltro
                        OR
                        TipoLista like @textoFiltro
                        OR
                        IdPai like @textoFiltro
                    ) 
					AS 'totRegistrosFiltro'

	                FROM ItemLista
						WHERE
                        Id like @textoFiltro
                        OR
                        Nome collate Latin1_General_CI_AI like @textoFiltro
                        OR
                        TipoLista like @textoFiltro
                        OR
                        IdPai like @textoFiltro) 

				AS todasLinhas
                WHERE todasLinhas.numeroLinha > (@start)"); 

                comm.Parameters.Add(new SqlParameter("pageSize", pageSize));
                comm.Parameters.Add(new SqlParameter("start", start));
                comm.Parameters.Add(new SqlParameter("textoFiltro", string.Format("%{0}%", textoFiltro)));

                comm.CommandText = queryGet.ToString();

                con.Open();

                SqlDataReader rd = comm.ExecuteReader();

                ItemListaTO obj;

                if (rd.Read())
                {
                    totRegistros = rd.GetInt32(5);
                    totRegistrosFiltro = rd.GetInt32(6);

                    obj = new ItemListaTO
                    {
                        Id = rd.GetInt32(0),
                        Nome = rd.GetString(1),
                        TipoLista = rd.GetInt32(2),
                        IdPai = rd.IsDBNull(3) ? (Int32?)null : rd.GetInt32(3)
                    };
                    objs.Add(obj);
                }
                while (rd.Read())
                {
                    obj = new ItemListaTO
                    {
                        Id = rd.GetInt32(0),
                        Nome = rd.GetString(1),
                        TipoLista = rd.GetInt32(2),
                        IdPai = rd.IsDBNull(3) ? (Int32?)null : rd.GetInt32(3)
                    };
                    objs.Add(obj);
                }
                rd.Close();
            }
            catch (Exception ex)
            {
                objs.Clear();
            }
            finally
            {
                con.Close();
            }

            return objs;
        }
        
        public static IList<object> GetParaChaveEstrangeira()
        {
            IList<object> objs = new List<object>();

            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                StringBuilder queryGet = new StringBuilder(@"
                SELECT
                chaveRegistro = CONVERT(VARCHAR, Id),
                valorRegistro = CONVERT(VARCHAR, Id)

                FROM ItemLista

                ORDER BY valorRegistro");

                comm.CommandText = queryGet.ToString();

                con.Open();

                SqlDataReader rd = comm.ExecuteReader();

                object obj;

                while (rd.Read())
                {
                    obj = new 
                    {
                        chaveRegistro = rd.GetString(0),
                        valorRegistro = rd.GetString(1)
                    };
                    objs.Add(obj);
                }
                rd.Close();
            }
            catch (Exception ex)
            {
                objs.Clear();
            }
            finally
            {
                con.Close();
            }

            return objs;
        }
        
        public static int? Insert(ItemListaTO obj)
        {
            int? nrLinhas;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;
            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;
                comm.CommandText = @"
                INSERT INTO ItemLista 
                (Nome, TipoLista, IdPai) VALUES 
                (@Nome, @TipoLista, @IdPai)
                ";

                con.Open();

                object IdPai = DBNull.Value;
                if (null != obj.IdPai)
                {
                    IdPai = obj.IdPai;
                }

                comm.Parameters.Add(new SqlParameter("Nome", obj.Nome));
                comm.Parameters.Add(new SqlParameter("TipoLista", obj.TipoLista));
                comm.Parameters.Add(new SqlParameter("IdPai", IdPai));

                nrLinhas = comm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                nrLinhas = null;
            }
            finally
            {
                con.Close();
            }
            return nrLinhas;
        }
                
        public static int? Update(ItemListaTO obj)
        {
            int? nrLinhas;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;
            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;
                comm.CommandText = @"
                UPDATE ItemLista 
                SET Nome = @Nome,
                TipoLista = @TipoLista,
                IdPai = @IdPai
                WHERE Id = @Id
                ";

                con.Open();

                object IdPai = DBNull.Value;
                if (null != obj.IdPai)
                {
                    IdPai = obj.IdPai;
                }

                comm.Parameters.Add(new SqlParameter("Nome", obj.Nome));
                comm.Parameters.Add(new SqlParameter("TipoLista", obj.TipoLista));
                comm.Parameters.Add(new SqlParameter("IdPai", IdPai));
                comm.Parameters.Add(new SqlParameter("Id", obj.Id));

                nrLinhas = comm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                nrLinhas = null;
            }
            finally
            {
                con.Close();
            }
            return nrLinhas;
        }
                
        public static int? Delete(ItemListaTO obj)
        {
            int? nrLinhas;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;
            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;
                comm.CommandText = @"
                DELETE ItemLista 
                WHERE Id = @Id
                ";

                con.Open();
        
                comm.Parameters.Add(new SqlParameter("Id", obj.Id));

                nrLinhas = comm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                nrLinhas = null;
            }
            finally
            {
                con.Close();
            }
            return nrLinhas;
        }
    }
}