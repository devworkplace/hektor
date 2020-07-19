using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using CrudGeneratorTest.Models.TO;

namespace CrudGeneratorTest.Models.DAL
{
    public class PessoaDAL
    {
        public static IList<PessoaTO> Get(int start, int pageSize, ref int totRegistros, string textoFiltro, ref int totRegistrosFiltro, string sortColumn, string sortColumnDir)
        {
            IList<PessoaTO> objs = new List<PessoaTO>();

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
                    Sobrenome,

                    (ROW_NUMBER() OVER (").Append(ordenacao).Append(@"))
                    AS 'numeroLinha', 

                    (SELECT COUNT(Id) FROM Pessoa) 
				    AS 'totRegistros', 

					(SELECT COUNT(Id) FROM Pessoa 
					    WHERE
                        Id like @textoFiltro
                        OR
                        Nome collate Latin1_General_CI_AI like @textoFiltro
                        OR
                        Sobrenome collate Latin1_General_CI_AI like @textoFiltro
                    ) 
					AS 'totRegistrosFiltro'

	                FROM Pessoa
						WHERE
                        Id like @textoFiltro
                        OR
                        Nome collate Latin1_General_CI_AI like @textoFiltro
                        OR
                        Sobrenome collate Latin1_General_CI_AI like @textoFiltro) 

				AS todasLinhas
                WHERE todasLinhas.numeroLinha > (@start)"); 

                comm.Parameters.Add(new SqlParameter("pageSize", pageSize));
                comm.Parameters.Add(new SqlParameter("start", start));
                comm.Parameters.Add(new SqlParameter("textoFiltro", string.Format("%{0}%", textoFiltro)));

                comm.CommandText = queryGet.ToString();

                con.Open();

                SqlDataReader rd = comm.ExecuteReader();

                PessoaTO obj;

                if (rd.Read())
                {
                    totRegistros = rd.GetInt32(4);
                    totRegistrosFiltro = rd.GetInt32(5);

                    obj = new PessoaTO
                    {
                        Id = rd.GetInt32(0),
                        Nome = rd.GetString(1),
                        Sobrenome = rd.IsDBNull(2) ? null : rd.GetString(2)
                    };
                    objs.Add(obj);
                }
                while (rd.Read())
                {
                    obj = new PessoaTO
                    {
                        Id = rd.GetInt32(0),
                        Nome = rd.GetString(1),
                        Sobrenome = rd.IsDBNull(2) ? null : rd.GetString(2)
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
        
        public static int? Insert(PessoaTO obj)
        {
            int? nrLinhas;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;
            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;
                comm.CommandText = @"
                INSERT INTO Pessoa 
                (Nome, Sobrenome) VALUES 
                (@Nome, @Sobrenome)
                ";

                con.Open();

                object Sobrenome = DBNull.Value;
                if (null != obj.Sobrenome)
                {
                    Sobrenome = obj.Sobrenome;
                }

                comm.Parameters.Add(new SqlParameter("Nome", obj.Nome));
                comm.Parameters.Add(new SqlParameter("Sobrenome", Sobrenome));

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
                
        public static int? Update(PessoaTO obj)
        {
            int? nrLinhas;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;
            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;
                comm.CommandText = @"
                UPDATE Pessoa 
                SET Nome = @Nome,
                Sobrenome = @Sobrenome
                WHERE Id = @Id
                ";

                con.Open();

                object Sobrenome = DBNull.Value;
                if (null != obj.Sobrenome)
                {
                    Sobrenome = obj.Sobrenome;
                }

                comm.Parameters.Add(new SqlParameter("Nome", obj.Nome));
                comm.Parameters.Add(new SqlParameter("Sobrenome", Sobrenome));
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
                
        public static int? Delete(PessoaTO obj)
        {
            int? nrLinhas;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;
            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;
                comm.CommandText = @"
                DELETE Pessoa 
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