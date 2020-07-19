using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using CrudGeneratorTest.Models.TO;

namespace CrudGeneratorTest.Models.DAL
{
    public class TelaDAL
    {
        public static IList<TelaTO> Get(int start, int pageSize, ref int totRegistros, string textoFiltro, ref int totRegistrosFiltro, string sortColumn, string sortColumnDir)
        {
            IList<TelaTO> objs = new List<TelaTO>();

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

                    (SELECT COUNT(Id) FROM Tela) 
				    AS 'totRegistros', 

					(SELECT COUNT(Id) FROM Tela 
					    WHERE
                        Id like @textoFiltro
                        OR
                        OrigemFluxo collate Latin1_General_CI_AI like @textoFiltro
                        OR
                        DestinoFluxo collate Latin1_General_CI_AI like @textoFiltro
                    ) 
					AS 'totRegistrosFiltro'

	                FROM Tela
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

                TelaTO obj;

                if (rd.Read())
                {
                    totRegistros = rd.GetInt32(4);
                    totRegistrosFiltro = rd.GetInt32(5);

                    obj = new TelaTO
                    {
                        Id = rd.GetInt32(0),
                        OrigemFluxo = rd.GetString(1),
                        DestinoFluxo = rd.GetString(2)
                    };
                    objs.Add(obj);
                }
                while (rd.Read())
                {
                    obj = new TelaTO
                    {
                        Id = rd.GetInt32(0),
                        OrigemFluxo = rd.GetString(1),
                        DestinoFluxo = rd.GetString(2)
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
        
        public static int? Insert(TelaTO obj)
        {
            int? nrLinhas;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;
            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;
                comm.CommandText = @"
                INSERT INTO Tela 
                (OrigemFluxo, DestinoFluxo) VALUES 
                (@OrigemFluxo, @DestinoFluxo)
                ";

                con.Open();

                comm.Parameters.Add(new SqlParameter("OrigemFluxo", obj.OrigemFluxo));
                comm.Parameters.Add(new SqlParameter("DestinoFluxo", obj.DestinoFluxo));

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
                
        public static int? Update(TelaTO obj)
        {
            int? nrLinhas;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;
            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;
                comm.CommandText = @"
                UPDATE Tela 
                SET OrigemFluxo = @OrigemFluxo,
                DestinoFluxo = @DestinoFluxo
                WHERE Id = @Id
                ";

                con.Open();

                comm.Parameters.Add(new SqlParameter("OrigemFluxo", obj.OrigemFluxo));
                comm.Parameters.Add(new SqlParameter("DestinoFluxo", obj.DestinoFluxo));
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
                
        public static int? Delete(TelaTO obj)
        {
            int? nrLinhas;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;
            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;
                comm.CommandText = @"
                DELETE Tela 
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