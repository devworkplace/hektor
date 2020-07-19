using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using CrudGeneratorTest.Models.TO;

namespace CrudGeneratorTest.Models.DAL
{
    public class FuncionarioDAL
    {
        public static IList<FuncionarioTO> Get(int start, int pageSize, ref int totRegistros, string textoFiltro, ref int totRegistrosFiltro, string sortColumn, string sortColumnDir)
        {
            IList<FuncionarioTO> objs = new List<FuncionarioTO>();

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
                    Departamento,

                    (ROW_NUMBER() OVER (").Append(ordenacao).Append(@"))
                    AS 'numeroLinha', 

                    (SELECT COUNT(Id) FROM Funcionario) 
				    AS 'totRegistros', 

					(SELECT COUNT(Id) FROM Funcionario 
					    WHERE
                        Id like @textoFiltro
                        OR
                        Nome collate Latin1_General_CI_AI like @textoFiltro
                        OR
                        Sobrenome collate Latin1_General_CI_AI like @textoFiltro
                        OR
                        Departamento like @textoFiltro
                    ) 
					AS 'totRegistrosFiltro'

	                FROM Funcionario
						WHERE
                        Id like @textoFiltro
                        OR
                        Nome collate Latin1_General_CI_AI like @textoFiltro
                        OR
                        Sobrenome collate Latin1_General_CI_AI like @textoFiltro
                        OR
                        Departamento like @textoFiltro) 

				AS todasLinhas
                WHERE todasLinhas.numeroLinha > (@start)"); 

                comm.Parameters.Add(new SqlParameter("pageSize", pageSize));
                comm.Parameters.Add(new SqlParameter("start", start));
                comm.Parameters.Add(new SqlParameter("textoFiltro", string.Format("%{0}%", textoFiltro)));

                comm.CommandText = queryGet.ToString();

                con.Open();

                SqlDataReader rd = comm.ExecuteReader();

                FuncionarioTO obj;

                if (rd.Read())
                {
                    totRegistros = rd.GetInt32(5);
                    totRegistrosFiltro = rd.GetInt32(6);

                    obj = new FuncionarioTO
                    {
                        Id = rd.GetInt32(0),
                        Nome = rd.GetString(1),
                        Sobrenome = rd.GetString(2),
                        Departamento = rd.GetInt32(3)
                    };
                    objs.Add(obj);
                }
                while (rd.Read())
                {
                    obj = new FuncionarioTO
                    {
                        Id = rd.GetInt32(0),
                        Nome = rd.GetString(1),
                        Sobrenome = rd.GetString(2),
                        Departamento = rd.GetInt32(3)
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
        
        public static int? Insert(FuncionarioTO obj)
        {
            int? nrLinhas;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;
            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;
                comm.CommandText = @"
                INSERT INTO Funcionario 
                (Nome, Sobrenome, Departamento) VALUES 
                (@Nome, @Sobrenome, @Departamento)
                ";

                con.Open();

                comm.Parameters.Add(new SqlParameter("Nome", obj.Nome));
                comm.Parameters.Add(new SqlParameter("Sobrenome", obj.Sobrenome));
                comm.Parameters.Add(new SqlParameter("Departamento", obj.Departamento));

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
                
        public static int? Update(FuncionarioTO obj)
        {
            int? nrLinhas;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;
            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;
                comm.CommandText = @"
                UPDATE Funcionario 
                SET Nome = @Nome,
                Sobrenome = @Sobrenome,
                Departamento = @Departamento
                WHERE Id = @Id
                ";

                con.Open();

                comm.Parameters.Add(new SqlParameter("Nome", obj.Nome));
                comm.Parameters.Add(new SqlParameter("Sobrenome", obj.Sobrenome));
                comm.Parameters.Add(new SqlParameter("Departamento", obj.Departamento));
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
                
        public static int? Delete(FuncionarioTO obj)
        {
            int? nrLinhas;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;
            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;
                comm.CommandText = @"
                DELETE Funcionario 
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