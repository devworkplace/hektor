using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using CrudGeneratorTest.Models.TO;

namespace CrudGeneratorTest.Models.DAL
{
    public class FormularioDAL
    {
        public static IList<FormularioTO> Get(int start, int pageSize, ref int totRegistros, string textoFiltro, ref int totRegistrosFiltro, string sortColumn, string sortColumnDir)
        {
            IList<FormularioTO> objs = new List<FormularioTO>();

            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                string ordenacao;
                if (string.IsNullOrEmpty(sortColumn))
                {
                    ordenacao = "ORDER BY id";
                }
                else
                {
                    ordenacao = string.Format("ORDER BY {0} {1}", sortColumn, sortColumnDir);
                }
                StringBuilder queryGet = new StringBuilder(@"
                SELECT TOP (@pageSize) *
                FROM (
				    SELECT 
                    id,
                    codigo,
                    nome,

                    (ROW_NUMBER() OVER (").Append(ordenacao).Append(@"))
                    AS 'numeroLinha', 

                    (SELECT COUNT(id) FROM Formulario) 
				    AS 'totRegistros', 

					(SELECT COUNT(id) FROM Formulario 
					    WHERE
                        id like @textoFiltro
                        OR
                        codigo like @textoFiltro
                        OR
                        nome collate Latin1_General_CI_AI like @textoFiltro
                    ) 
					AS 'totRegistrosFiltro'

	                FROM Formulario
						WHERE
                        id like @textoFiltro
                        OR
                        codigo like @textoFiltro
                        OR
                        nome collate Latin1_General_CI_AI like @textoFiltro) 

				AS todasLinhas
                WHERE todasLinhas.numeroLinha > (@start)");

                comm.Parameters.Add(new SqlParameter("pageSize", pageSize));
                comm.Parameters.Add(new SqlParameter("start", start));
                comm.Parameters.Add(new SqlParameter("textoFiltro", string.Format("%{0}%", textoFiltro)));

                comm.CommandText = queryGet.ToString();

                con.Open();

                SqlDataReader rd = comm.ExecuteReader();

                FormularioTO obj;

                if (rd.Read())
                {
                    totRegistros = rd.GetInt32(4);
                    totRegistrosFiltro = rd.GetInt32(5);

                    obj = new FormularioTO
                    {
                        id = rd.GetInt32(0),
                        codigo = rd.GetInt32(1),
                        nome = rd.GetString(2)
                    };
                    objs.Add(obj);
                }
                while (rd.Read())
                {
                    obj = new FormularioTO
                    {
                        id = rd.GetInt32(0),
                        codigo = rd.GetInt32(1),
                        nome = rd.GetString(2)
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

        public static int? Insert(FormularioTO obj)
        {
            int? nrLinhas;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;
            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;
                comm.CommandText = @"
                INSERT INTO Formulario 
                (codigo, nome) VALUES 
                (@codigo, @nome)
                ";

                con.Open();

                comm.Parameters.Add(new SqlParameter("codigo", obj.codigo));
                comm.Parameters.Add(new SqlParameter("nome", obj.nome));

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

        public static int? Update(FormularioTO obj)
        {
            int? nrLinhas;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;
            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;
                comm.CommandText = @"
                UPDATE Formulario 
                SET codigo = @codigo,
                nome = @nome
                WHERE id = @id
                ";

                con.Open();

                comm.Parameters.Add(new SqlParameter("codigo", obj.codigo));
                comm.Parameters.Add(new SqlParameter("nome", obj.nome));
                comm.Parameters.Add(new SqlParameter("id", obj.id));

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

        public static int? Delete(FormularioTO obj)
        {
            int? nrLinhas;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;
            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;
                comm.CommandText = @"
                DELETE Formulario 
                WHERE id = @id
                ";

                con.Open();

                comm.Parameters.Add(new SqlParameter("id", obj.id));

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