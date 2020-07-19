using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CrudGeneratorForm
{
    public class Util
    {
        private string stringConexao;

        private Util()
        {

        }

        public Util(string stringConexao)
        {
            this.stringConexao = stringConexao;
        }

        public void PovoarCbxTabelas(CheckedListBox cbxTabelas)
        {
            SqlConnection con = new SqlConnection();
            con.ConnectionString = this.stringConexao;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                StringBuilder queryGet = new StringBuilder(@"
                SELECT TABLE_NAME
                    FROM INFORMATION_SCHEMA.TABLES
                ");

                comm.CommandText = queryGet.ToString();

                con.Open();

                SqlDataReader rd = comm.ExecuteReader();

                while (rd.Read())
                {
                    cbxTabelas.Items.Add(rd.GetString(0));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        public IList<Coluna> GetColunasTabela(string nomeTabela)
        {
            IList<Coluna> result;

            SqlConnection con = new SqlConnection();
            con.ConnectionString = this.stringConexao;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                StringBuilder queryGet = new StringBuilder(@"
                SELECT 
                nome = co.name, 
                tipo = tp.name,
                obrigatoriedade = co.is_nullable, 
                autoIncremento = co.is_identity,
                indice = ob.type,
                tamanho = co.max_length

                FROM sys.tables tb
                JOIN sys.columns co ON co.object_id = tb.object_id
                JOIN sys.types tp ON tp.system_type_id = co.system_type_id
                LEFT JOIN sys.index_columns ic ON ic.object_id = tb.object_id AND ic.column_id = co.column_id
                LEFT JOIN sys.indexes ix ON ix.object_id = tb.object_id AND ix.index_id = ic.index_id
                LEFT JOIN sys.objects ob ON ob.name = ix.name

                WHERE tb.name = @TABLE_NAME
                ORDER BY co.column_id
                ");

                comm.CommandText = queryGet.ToString();

                con.Open();

                comm.Parameters.Add(new SqlParameter("TABLE_NAME", nomeTabela));

                SqlDataReader rd = comm.ExecuteReader();

                result = new List<Coluna>();

                while (rd.Read())
                {
                    result.Add(new Coluna
                    (
                        rd.GetString(rd.GetOrdinal("nome")),
                        rd.GetString(rd.GetOrdinal("tipo")),
                        !rd.GetBoolean(rd.GetOrdinal("obrigatoriedade")),
                        rd.GetBoolean(rd.GetOrdinal("autoIncremento")),
                        rd.IsDBNull(rd.GetOrdinal("indice")) ? null : rd.GetString(rd.GetOrdinal("indice")),
                        rd.GetInt16(rd.GetOrdinal("tamanho"))
                    ));
                }
            }
            catch (Exception ex)
            {
                result = null;
            }
            finally
            {
                con.Close();
            }
            return result;
        }

        public IList<ChaveEstrangeira> GetChavesEstrangeiras(string nomeTabela)
        {
            IList<ChaveEstrangeira> result;

            SqlConnection con = new SqlConnection();
            con.ConnectionString = this.stringConexao;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                StringBuilder queryGet = new StringBuilder(@"
                SELECT 
                foreign_key = fk.name, 
                table_name = OBJECT_NAME(fk.parent_object_id), 
                column_name = COL_NAME(fkc.parent_object_id, fkc.parent_column_id), 
                reference_table_name = OBJECT_NAME (fk.referenced_object_id), 
                reference_column_name = COL_NAME(fkc.referenced_object_id, fkc.referenced_column_id) 
                
                FROM sys.tables tb 
                JOIN sys.foreign_keys AS fk ON fk.parent_object_id = tb.object_id
                JOIN sys.foreign_key_columns AS fkc ON fk.OBJECT_ID = fkc.constraint_object_id
                
                WHERE tb.name = @TABLE_NAME
                
                ORDER BY foreign_key
                ");

                comm.CommandText = queryGet.ToString();

                con.Open();

                comm.Parameters.Add(new SqlParameter("TABLE_NAME", nomeTabela));

                SqlDataReader rd = comm.ExecuteReader();

                result = new List<ChaveEstrangeira>();

                String nomeChave = string.Empty;
                String leituraNomeChave = string.Empty;
                String tabelaDestino = string.Empty;
                String leituraTabelaDestino = string.Empty;
                String colunaOrigem;
                String colunaDestino;
                IDictionary<String, String> colunasChave = new Dictionary<String, String>();
                while (rd.Read())
                {
                    colunaOrigem = rd.GetString(rd.GetOrdinal("column_name"));
                    colunaDestino = rd.GetString(rd.GetOrdinal("reference_column_name"));

                    leituraNomeChave = rd.GetString(rd.GetOrdinal("foreign_key"));
                    leituraTabelaDestino = rd.GetString(rd.GetOrdinal("reference_table_name"));
                    if (!leituraNomeChave.Equals(nomeChave))
                    {
                        if (!string.IsNullOrEmpty(nomeChave))
                        {
                            result.Add(new ChaveEstrangeira(tabelaDestino, colunasChave));
                            colunasChave = new Dictionary<String, String>();
                        }
                        nomeChave = leituraNomeChave;
                        tabelaDestino = leituraTabelaDestino;
                    }

                    colunasChave.Add(colunaOrigem, colunaDestino);
                }
                if (!string.IsNullOrEmpty(tabelaDestino))
                {
                    result.Add(new ChaveEstrangeira(tabelaDestino, colunasChave));
                }
            }
            catch (Exception ex)
            {
                result = null;
            }
            finally
            {
                con.Close();
            }
            return result;
        }

        public bool TabelaReferenciada(string nomeTabela)
        {
            SqlConnection con = new SqlConnection();
            con.ConnectionString = this.stringConexao;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                StringBuilder queryGet = new StringBuilder(@"
                EXEC sp_fkeys @TABLE_NAME
                ");

                comm.CommandText = queryGet.ToString();

                con.Open();

                comm.Parameters.Add(new SqlParameter("TABLE_NAME", nomeTabela));

                SqlDataReader rd = comm.ExecuteReader();

                return rd.Read();
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                con.Close();
            }
        }

        public String FormatarDataParaDiretorioRaiz()
        {
            StringBuilder result = new StringBuilder();
            DateTime dataHora = DateTime.Now;
            result.AppendFormat("{0:00}{1:00}{2}{3:00}{4:00}{5:00}", 
                dataHora.Day, dataHora.Month, dataHora.Year,
                dataHora.Hour, dataHora.Minute, dataHora.Second);
            return result.ToString();
        }

        public DirectoryInfo CriarDiretorio(string path)
        {
            DirectoryInfo result;
            try
            {
                result = Directory.CreateDirectory(path);
            }
            catch (Exception ex)
            {
                result = null;
                MessageBox.Show(ex.Message);
            }
            return result;
        }

        public void LimparDiretorio(string path)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                    Directory.CreateDirectory(path);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
