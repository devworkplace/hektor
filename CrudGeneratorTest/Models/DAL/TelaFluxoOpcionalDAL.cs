using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using CrudGeneratorTest.Models.TO;

namespace CrudGeneratorTest.Models.DAL
{
    public class TelaFluxoOpcionalDAL
    {
        public static IList<TelaFluxoOpcionalTO> Get(int start, int pageSize, ref int totRegistros, string textoFiltro, ref int totRegistrosFiltro, string sortColumn, string sortColumnDir)
        {
            IList<TelaFluxoOpcionalTO> objs = new List<TelaFluxoOpcionalTO>();

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
                    OrigemFluxo,
                    DestinoFluxo,

                    (ROW_NUMBER() OVER (").Append(ordenacao).Append(@"))
                    AS 'numeroLinha', 

                    (SELECT COUNT(Id) FROM TelaFluxoOpcional) 
				    AS 'totRegistros', 

					(SELECT COUNT(Id) FROM TelaFluxoOpcional 
					    WHERE
                        Id like @textoFiltro
                        OR
                        OrigemFluxo collate Latin1_General_CI_AI like @textoFiltro
                        OR
                        DestinoFluxo collate Latin1_General_CI_AI like @textoFiltro
                    ) 
					AS 'totRegistrosFiltro'

	                FROM TelaFluxoOpcional
						WHERE
                        Id like @textoFiltro
                        OR
                        OrigemFluxo collate Latin1_General_CI_AI like @textoFiltro
                        OR
                        DestinoFluxo collate Latin1_General_CI_AI like @textoFiltro) 

				AS todasLinhas
                WHERE todasLinhas.numeroLinha > (@start)"); 

                comm.Parameters.Add(new SqlParameter("pageSize", pageSize));
                comm.Parameters.Add(new SqlParameter("start", start));
                comm.Parameters.Add(new SqlParameter("textoFiltro", string.Format("%{0}%", textoFiltro)));

                comm.CommandText = queryGet.ToString();

                con.Open();

                SqlDataReader rd = comm.ExecuteReader();

                TelaFluxoOpcionalTO obj;

                if (rd.Read())
                {
                    totRegistros = rd.GetInt32(4);
                    totRegistrosFiltro = rd.GetInt32(5);

                    obj = new TelaFluxoOpcionalTO
                    {
                        Id = rd.GetInt32(0),
                        OrigemFluxo = rd.IsDBNull(1) ? null : rd.GetString(1),
                        DestinoFluxo = rd.IsDBNull(2) ? null : rd.GetString(2)
                    };
                    objs.Add(obj);
                }
                while (rd.Read())
                {
                    obj = new TelaFluxoOpcionalTO
                    {
                        Id = rd.GetInt32(0),
                        OrigemFluxo = rd.IsDBNull(1) ? null : rd.GetString(1),
                        DestinoFluxo = rd.IsDBNull(2) ? null : rd.GetString(2)
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
        
        public static int? Insert(TelaFluxoOpcionalTO obj)
        {
            int? nrLinhas;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;
            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;
                comm.CommandText = @"
                INSERT INTO TelaFluxoOpcional 
                (OrigemFluxo, DestinoFluxo) VALUES 
                (@OrigemFluxo, @DestinoFluxo)
                ";

                con.Open();

                object OrigemFluxo = DBNull.Value;
                if (null != obj.OrigemFluxo)
                {
                    OrigemFluxo = obj.OrigemFluxo;
                }

                object DestinoFluxo = DBNull.Value;
                if (null != obj.DestinoFluxo)
                {
                    DestinoFluxo = obj.DestinoFluxo;
                }

                comm.Parameters.Add(new SqlParameter("OrigemFluxo", OrigemFluxo));
                comm.Parameters.Add(new SqlParameter("DestinoFluxo", DestinoFluxo));

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
                
        public static int? Update(TelaFluxoOpcionalTO obj)
        {
            int? nrLinhas;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;
            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;
                comm.CommandText = @"
                UPDATE TelaFluxoOpcional 
                SET OrigemFluxo = @OrigemFluxo,
                DestinoFluxo = @DestinoFluxo
                WHERE Id = @Id
                ";

                con.Open();

                object OrigemFluxo = DBNull.Value;
                if (null != obj.OrigemFluxo)
                {
                    OrigemFluxo = obj.OrigemFluxo;
                }

                object DestinoFluxo = DBNull.Value;
                if (null != obj.DestinoFluxo)
                {
                    DestinoFluxo = obj.DestinoFluxo;
                }

                comm.Parameters.Add(new SqlParameter("OrigemFluxo", OrigemFluxo));
                comm.Parameters.Add(new SqlParameter("DestinoFluxo", DestinoFluxo));
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
                
        public static int? Delete(TelaFluxoOpcionalTO obj)
        {
            int? nrLinhas;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;
            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;
                comm.CommandText = @"
                DELETE TelaFluxoOpcional 
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