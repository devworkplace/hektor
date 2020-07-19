using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using CrudGeneratorTest.Models.TO;

namespace CrudGeneratorTest.Models.DAL
{
    public class TB_ITEM_LISTADAL
    {
        public static IList<TB_ITEM_LISTATO> Get(int start, int pageSize, ref int totRegistros, string textoFiltro, ref int totRegistrosFiltro, string sortColumn, string sortColumnDir)
        {
            IList<TB_ITEM_LISTATO> objs = new List<TB_ITEM_LISTATO>();

            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                string ordenacao;
                if (string.IsNullOrEmpty(sortColumn))
                {
                    ordenacao = "ORDER BY ID";
                }
                else
                {
                    ordenacao = string.Format("ORDER BY {0} {1}", sortColumn, sortColumnDir);
                }
                StringBuilder queryGet = new StringBuilder(@"
                SELECT TOP (@pageSize) *
                FROM (
				    SELECT 
                    ID,
                    NOME,
                    ID_TIPO_LISTA,
                    ID_PAI,

                    (ROW_NUMBER() OVER (").Append(ordenacao).Append(@"))
                    AS 'numeroLinha', 

                    (SELECT COUNT(ID) FROM TB_ITEM_LISTA) 
				    AS 'totRegistros', 

					(SELECT COUNT(ID) FROM TB_ITEM_LISTA 
					    WHERE
                        ID like @textoFiltro
                        OR
                        NOME collate Latin1_General_CI_AI like @textoFiltro
                        OR
                        ID_TIPO_LISTA like @textoFiltro
                        OR
                        ID_PAI like @textoFiltro
                    ) 
					AS 'totRegistrosFiltro'

	                FROM TB_ITEM_LISTA
						WHERE
                        ID like @textoFiltro
                        OR
                        NOME collate Latin1_General_CI_AI like @textoFiltro
                        OR
                        ID_TIPO_LISTA like @textoFiltro
                        OR
                        ID_PAI like @textoFiltro) 

				AS todasLinhas
                WHERE todasLinhas.numeroLinha > (@start)"); 

                comm.Parameters.Add(new SqlParameter("pageSize", pageSize));
                comm.Parameters.Add(new SqlParameter("start", start));
                comm.Parameters.Add(new SqlParameter("textoFiltro", string.Format("%{0}%", textoFiltro)));

                comm.CommandText = queryGet.ToString();

                con.Open();

                SqlDataReader rd = comm.ExecuteReader();

                TB_ITEM_LISTATO obj;

                if (rd.Read())
                {
                    totRegistros = rd.GetInt32(5);
                    totRegistrosFiltro = rd.GetInt32(6);

                    obj = new TB_ITEM_LISTATO
                    {
                        ID = rd.GetInt32(0),
                        NOME = rd.GetString(1),
                        ID_TIPO_LISTA = rd.GetInt32(2),
                        ID_PAI = rd.IsDBNull(3) ? (Int32?)null : rd.GetInt32(3)
                    };
                    objs.Add(obj);
                }
                while (rd.Read())
                {
                    obj = new TB_ITEM_LISTATO
                    {
                        ID = rd.GetInt32(0),
                        NOME = rd.GetString(1),
                        ID_TIPO_LISTA = rd.GetInt32(2),
                        ID_PAI = rd.IsDBNull(3) ? (Int32?)null : rd.GetInt32(3)
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
                chaveRegistro = CONVERT(VARCHAR, ID),
                valorRegistro = CONVERT(VARCHAR, ID)

                FROM TB_ITEM_LISTA

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
        
        public static int? Insert(TB_ITEM_LISTATO obj)
        {
            int? nrLinhas;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;
            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;
                comm.CommandText = @"
                INSERT INTO TB_ITEM_LISTA 
                (NOME, ID_TIPO_LISTA, ID_PAI) VALUES 
                (@NOME, @ID_TIPO_LISTA, @ID_PAI)
                ";

                con.Open();

                object ID_PAI = DBNull.Value;
                if (null != obj.ID_PAI)
                {
                    ID_PAI = obj.ID_PAI;
                }

                comm.Parameters.Add(new SqlParameter("NOME", obj.NOME));
                comm.Parameters.Add(new SqlParameter("ID_TIPO_LISTA", obj.ID_TIPO_LISTA));
                comm.Parameters.Add(new SqlParameter("ID_PAI", ID_PAI));

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
                
        public static int? Update(TB_ITEM_LISTATO obj)
        {
            int? nrLinhas;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;
            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;
                comm.CommandText = @"
                UPDATE TB_ITEM_LISTA 
                SET NOME = @NOME,
                ID_TIPO_LISTA = @ID_TIPO_LISTA,
                ID_PAI = @ID_PAI
                WHERE ID = @ID
                ";

                con.Open();

                object ID_PAI = DBNull.Value;
                if (null != obj.ID_PAI)
                {
                    ID_PAI = obj.ID_PAI;
                }

                comm.Parameters.Add(new SqlParameter("NOME", obj.NOME));
                comm.Parameters.Add(new SqlParameter("ID_TIPO_LISTA", obj.ID_TIPO_LISTA));
                comm.Parameters.Add(new SqlParameter("ID_PAI", ID_PAI));
                comm.Parameters.Add(new SqlParameter("ID", obj.ID));

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
                
        public static int? Delete(TB_ITEM_LISTATO obj)
        {
            int? nrLinhas;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;
            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;
                comm.CommandText = @"
                DELETE TB_ITEM_LISTA 
                WHERE ID = @ID
                ";

                con.Open();
        
                comm.Parameters.Add(new SqlParameter("ID", obj.ID));

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