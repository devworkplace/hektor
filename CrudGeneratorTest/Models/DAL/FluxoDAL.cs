using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using CrudGeneratorTest.Models.TO;

namespace CrudGeneratorTest.Models.DAL
{
    public class FluxoDAL
    {
        public static IList<FluxoTO> Get(int start, int pageSize, ref int totRegistros, string textoFiltro, ref int totRegistrosFiltro, string sortColumn, string sortColumnDir)
        {
            IList<FluxoTO> objs = new List<FluxoTO>();

            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                string ordenacao;
                if (string.IsNullOrEmpty(sortColumn))
                {
                    ordenacao = "ORDER BY Origem";
                }
                else
                {
                    ordenacao = string.Format("ORDER BY {0} {1}", sortColumn, sortColumnDir);
                }
                StringBuilder queryGet = new StringBuilder(@"
                SELECT TOP (@pageSize) *
                FROM (
				    SELECT 
                    Origem,
                    Destino,

                    (ROW_NUMBER() OVER (").Append(ordenacao).Append(@"))
                    AS 'numeroLinha', 

                    (SELECT COUNT(Origem) FROM Fluxo) 
				    AS 'totRegistros', 

					(SELECT COUNT(Origem) FROM Fluxo 
					    WHERE
                        Origem collate Latin1_General_CI_AI like @textoFiltro
                        OR
                        Destino collate Latin1_General_CI_AI like @textoFiltro
                    ) 
					AS 'totRegistrosFiltro'

	                FROM Fluxo
						WHERE
                        Origem collate Latin1_General_CI_AI like @textoFiltro
                        OR
                        Destino collate Latin1_General_CI_AI like @textoFiltro) 

				AS todasLinhas
                WHERE todasLinhas.numeroLinha > (@start)"); 

                comm.Parameters.Add(new SqlParameter("pageSize", pageSize));
                comm.Parameters.Add(new SqlParameter("start", start));
                comm.Parameters.Add(new SqlParameter("textoFiltro", string.Format("%{0}%", textoFiltro)));

                comm.CommandText = queryGet.ToString();

                con.Open();

                SqlDataReader rd = comm.ExecuteReader();

                FluxoTO obj;

                if (rd.Read())
                {
                    totRegistros = rd.GetInt32(3);
                    totRegistrosFiltro = rd.GetInt32(4);

                    obj = new FluxoTO
                    {
                        Origem = rd.GetString(0),
                        Destino = rd.GetString(1)
                    };
                    objs.Add(obj);
                }
                while (rd.Read())
                {
                    obj = new FluxoTO
                    {
                        Origem = rd.GetString(0),
                        Destino = rd.GetString(1)
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
                chaveRegistro = CONVERT(VARCHAR, Origem + ';' + Destino),
                valorRegistro = CONVERT(VARCHAR, Origem + ' - ' + Destino)

                FROM Fluxo

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
        
        public static int? Insert(FluxoTO obj)
        {
            int? nrLinhas;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;
            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;
                comm.CommandText = @"
                INSERT INTO Fluxo 
                (Origem, Destino) VALUES 
                (@Origem, @Destino)
                ";

                con.Open();

                comm.Parameters.Add(new SqlParameter("Origem", obj.Origem));
                comm.Parameters.Add(new SqlParameter("Destino", obj.Destino));

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
                
        public static int? Update(FluxoTO obj)
        {
            int? nrLinhas;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;
            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;
                comm.CommandText = @"
                UPDATE Fluxo 
                
                WHERE Origem = @Origem
                AND Destino = @Destino
                ";

                con.Open();

                comm.Parameters.Add(new SqlParameter("Origem", obj.Origem));
                comm.Parameters.Add(new SqlParameter("Destino", obj.Destino));

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
                
        public static int? Delete(FluxoTO obj)
        {
            int? nrLinhas;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;
            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;
                comm.CommandText = @"
                DELETE Fluxo 
                WHERE Origem = @Origem
                AND Destino = @Destino
                ";

                con.Open();
        
                comm.Parameters.Add(new SqlParameter("Origem", obj.Origem));
                comm.Parameters.Add(new SqlParameter("Destino", obj.Destino));

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