using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using SETI.Models.TO;
using CrudGeneratorTest.Models.DAL;

namespace SETI.Models.DAL
{
    public class UsuarioDAL
    {
        public static IList<UsuarioTO> Get(int start, int pageSize, ref int totRegistros, string textoFiltro, ref int totRegistrosFiltro, string sortColumn, string sortColumnDir)
        {
            IList<UsuarioTO> objs = new List<UsuarioTO>();

            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                string ordenacao;
                if (string.IsNullOrEmpty(sortColumn))
                {
                    ordenacao = "ORDER BY usu.nome";
                }
                else
                {
                    string auxOrdenacao = string.Format("{0} {1}", sortColumn, sortColumnDir);
                    switch (auxOrdenacao)
                    {
                        case "cpf asc":
                            ordenacao = "ORDER BY usu.cpf";
                            break;
                        case "nome asc":
                            ordenacao = "ORDER BY usu.nome";
                            break;
                        case "email asc":
                            ordenacao = "ORDER BY usu.email";
                            break;
                        case "tipo asc":
                            ordenacao = "ORDER BY usu.tipo";
                            break;
                        case "nomeUsuarioCriador asc":
                            ordenacao = "ORDER BY usuarioCriador.nome";
                            break;
                        case "dataAdmissao asc":
                            ordenacao = "ORDER BY usu.dataAdmissao";
                            break;
                        case "cpf desc":
                            ordenacao = "ORDER BY usu.cpf DESC";
                            break;
                        case "nome desc":
                            ordenacao = "ORDER BY usu.nome DESC";
                            break;
                        case "email desc":
                            ordenacao = "ORDER BY usu.email DESC";
                            break;
                        case "tipo desc":
                            ordenacao = "ORDER BY usu.tipo DESC";
                            break;
                        case "nomeUsuarioCriador desc":
                            ordenacao = "ORDER BY usuarioCriador.nome DESC";
                            break;
                        case "dataAdmissao desc":
                            ordenacao = "ORDER BY usu.dataAdmissao DESC";
                            break;
                        default:
                            ordenacao = @"
                            ORDER BY usu.nome
                            ";
                            break;
                    }
                }
                StringBuilder queryGet = new StringBuilder(@"
                SELECT TOP (@pageSize) *
                FROM (
				    SELECT 
                    usu.id,
                    usu.login,
                    usu.senha,
                    usu.cpf,
                    usu.nome,
                    usu.email,
                    usu.tipo,
                    tipoGrid = (CASE WHEN 'B' = usu.tipo THEN 'Back Office' WHEN 'O' = usu.tipo THEN 'Operador' ELSE 'Supervisor' END),
                    idUsuarioCriador = usuarioCriador.id,
                    nomeUsuarioCriador = usuarioCriador.nome,
                    usu.dataAdmissao,
                    dataAdmissaoGrid = CONVERT(VARCHAR,usu.dataAdmissao,103),

                    (ROW_NUMBER() OVER (").Append(ordenacao).Append(@"))
                    AS 'numeroLinha', 

                    (SELECT COUNT(id) FROM Usuario) 
				    AS 'totRegistros', 

					(SELECT COUNT(usu.id) 
                    FROM Usuario usu
                    LEFT JOIN Usuario usuarioCriador ON usuarioCriador.id = usu.idUsuarioCriador
						WHERE
                        usu.cpf collate Latin1_General_CI_AI like @textoFiltro
                        OR
                        usu.nome collate Latin1_General_CI_AI like @textoFiltro
                        OR
                        usu.email collate Latin1_General_CI_AI like @textoFiltro
                        OR
                        (CASE WHEN 'B' = usu.tipo THEN 'Back Office' WHEN 'O' = usu.tipo THEN 'Operador' ELSE 'Supervisor' END) like @textoFiltro
                        OR
                        usuarioCriador.nome collate Latin1_General_CI_AI like @textoFiltro
                        OR
                        CONVERT(VARCHAR,usu.dataAdmissao,103) like @textoFiltro
                    ) 
					AS 'totRegistrosFiltro'

	                FROM Usuario usu
                    LEFT JOIN Usuario usuarioCriador ON usuarioCriador.id = usu.idUsuarioCriador
						WHERE
                        usu.cpf collate Latin1_General_CI_AI like @textoFiltro
                        OR
                        usu.nome collate Latin1_General_CI_AI like @textoFiltro
                        OR
                        usu.email collate Latin1_General_CI_AI like @textoFiltro
                        OR
                        (CASE WHEN 'B' = usu.tipo THEN 'Back Office' WHEN 'O' = usu.tipo THEN 'Operador' ELSE 'Supervisor' END) like @textoFiltro
                        OR
                        usuarioCriador.nome collate Latin1_General_CI_AI like @textoFiltro
                        OR
                        CONVERT(VARCHAR,usu.dataAdmissao,103) like @textoFiltro) 

				AS todasLinhas
                WHERE todasLinhas.numeroLinha > (@start)");

                comm.Parameters.Add(new SqlParameter("pageSize", pageSize));
                comm.Parameters.Add(new SqlParameter("start", start));
                comm.Parameters.Add(new SqlParameter("textoFiltro", string.Format("%{0}%", textoFiltro)));

                comm.CommandText = queryGet.ToString();

                con.Open();

                SqlDataReader rd = comm.ExecuteReader();

                UsuarioTO obj;

                if (rd.Read())
                {
                    totRegistros = rd.GetInt32(13);
                    totRegistrosFiltro = rd.GetInt32(14);

                    obj = new UsuarioTO
                    {
                        id = rd.GetInt32(0),
                        login = rd.GetString(1),
                        senha = rd.GetString(2),
                        cpf = rd.GetString(3),
                        nome = rd.GetString(4),
                        email = rd.GetString(5),
                        tipo = rd.GetString(6)[0],
                        tipoGrid = rd.GetString(7),
                        usuarioCriador = rd.IsDBNull(8) ? new UsuarioTO { nome = string.Empty } : new UsuarioTO { id = rd.GetInt32(8), nome = rd.GetString(9) },
                        dataAdmissao = rd.IsDBNull(10) ? (DateTime?)null : rd.GetDateTime(10),
                        dataAdmissaoGrid = rd.IsDBNull(11) ? (String)null : rd.GetString(11)
                    };
                    objs.Add(obj);
                }
                while (rd.Read())
                {
                    obj = new UsuarioTO
                    {
                        id = rd.GetInt32(0),
                        login = rd.GetString(1),
                        senha = rd.GetString(2),
                        cpf = rd.GetString(3),
                        nome = rd.GetString(4),
                        email = rd.GetString(5),
                        tipo = rd.GetString(6)[0],
                        tipoGrid = rd.GetString(7),
                        usuarioCriador = rd.IsDBNull(8) ? new UsuarioTO { nome = string.Empty } : new UsuarioTO { id = rd.GetInt32(8), nome = rd.GetString(9) },
                        dataAdmissao = rd.IsDBNull(10) ? (DateTime?)null : rd.GetDateTime(10),
                        dataAdmissaoGrid = rd.IsDBNull(11) ? (String)null : rd.GetString(11)
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
                chaveRegistro = CONVERT(VARCHAR, id),
                valorRegistro = nome

                FROM Usuario

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

        public static int? Insert(UsuarioTO obj)
        {
            int? nrLinhas;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;
            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;
                comm.CommandText = @"
                INSERT INTO Usuario 
                (login, senha, cpf, nome, email, tipo, idUsuarioCriador, dataAdmissao) VALUES 
                (@login, @senha, @cpf, @nome, @email, @tipo, @idUsuarioCriador, @dataAdmissao)
                ";

                con.Open();

                object idUsuarioCriador = DBNull.Value;
                if (null != obj.usuarioCriador)
                {
                    idUsuarioCriador = obj.usuarioCriador.id;
                }

                object dataAdmissao = DBNull.Value;
                if (null != obj.dataAdmissao)
                {
                    dataAdmissao = obj.dataAdmissao;
                }

                comm.Parameters.Add(new SqlParameter("login", obj.login));
                comm.Parameters.Add(new SqlParameter("senha", obj.senha));
                comm.Parameters.Add(new SqlParameter("cpf", obj.cpf));
                comm.Parameters.Add(new SqlParameter("nome", obj.nome));
                comm.Parameters.Add(new SqlParameter("email", obj.email));
                comm.Parameters.Add(new SqlParameter("tipo", obj.tipo));
                comm.Parameters.Add(new SqlParameter("idUsuarioCriador", idUsuarioCriador));
                comm.Parameters.Add(new SqlParameter("dataAdmissao", dataAdmissao));

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

        public static int? Update(UsuarioTO obj)
        {
            int? nrLinhas;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;
            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;
                comm.CommandText = @"
                UPDATE Usuario 
                SET login = @login,
                senha = @senha,
                cpf = @cpf,
                nome = @nome,
                email = @email,
                tipo = @tipo,
                idUsuarioCriador = @idUsuarioCriador,
                dataAdmissao = @dataAdmissao
                WHERE id = @id
                ";

                con.Open();

                object idUsuarioCriador = DBNull.Value;
                if (null != obj.usuarioCriador)
                {
                    idUsuarioCriador = obj.usuarioCriador.id;
                }

                object dataAdmissao = DBNull.Value;
                if (null != obj.dataAdmissao)
                {
                    dataAdmissao = obj.dataAdmissao;
                }

                comm.Parameters.Add(new SqlParameter("login", obj.login));
                comm.Parameters.Add(new SqlParameter("senha", obj.senha));
                comm.Parameters.Add(new SqlParameter("cpf", obj.cpf));
                comm.Parameters.Add(new SqlParameter("nome", obj.nome));
                comm.Parameters.Add(new SqlParameter("email", obj.email));
                comm.Parameters.Add(new SqlParameter("tipo", obj.tipo));
                comm.Parameters.Add(new SqlParameter("idUsuarioCriador", idUsuarioCriador));
                comm.Parameters.Add(new SqlParameter("dataAdmissao", dataAdmissao));
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

        public static int? Delete(UsuarioTO obj)
        {
            int? nrLinhas;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;
            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;
                comm.CommandText = @"
                DELETE Usuario 
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