using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CrudGeneratorForm
{
    public class Tabela
    {
        public string nomeNameSpace { get; }
        public string nome { get; }
        public string pathTO { get; }
        public string pathDAL { get; }
        public string pathController { get; }
        public string pathView { get; }
        public string pathScript { get; }
        public IList<Coluna> colunas { get; }
        public IList<ChaveEstrangeira> chavesEstrangeiras { get; }
        public bool tabelaReferenciada { get; }
        private Util util;

        private const string SIGLA_TO = "TO";
        private const string SIGLA_DAL = "DAL";
        private const string INDICE_PK = "PK";

        private Tabela()
        {

        }

        #region Common
        private ChaveEstrangeira ObterchaveEstrangeira(String nomeColunaOrigem)
        {
            IList<ChaveEstrangeira> result = this.chavesEstrangeiras.Where(ce => ce.colunasChave.ContainsKey(nomeColunaOrigem)).ToList();
            return (result != null && result.Count > 0) ? result[0] : null;
        }
        #endregion

        #region DAL
        private string ObterColunasInsertDAL(bool adicionarArroba)
        {
            StringBuilder colunasInsert = new StringBuilder();
            foreach (Coluna coluna in this.colunas.Where(coluna =>
                null != coluna &&
                !string.IsNullOrEmpty(coluna.nome) &&
                !coluna.autoIncremento))
            {
                if (string.IsNullOrEmpty(colunasInsert.ToString()))
                {
                    colunasInsert.Append(string.Format("{0}{1}", adicionarArroba ? "@" : string.Empty, coluna.nome));
                    continue;
                }
                colunasInsert.Append(string.Format(", {0}{1}", adicionarArroba ? "@" : string.Empty, coluna.nome));
            }
            return colunasInsert.ToString();
        }

        private string ObterColunasUpdateDAL()
        {
            StringBuilder colunasUpdate = new StringBuilder();
            foreach (Coluna coluna in this.colunas.Where(coluna =>
                null != coluna &&
                !string.IsNullOrEmpty(coluna.nome) &&
                !coluna.autoIncremento &&
                !INDICE_PK.Equals(coluna.indice)))
            {
                if (string.IsNullOrEmpty(colunasUpdate.ToString()))
                {
                    colunasUpdate.Append(string.Format("SET {0} = @{1}", coluna.nome, coluna.nome));
                    continue;
                }
                colunasUpdate.Append(string.Format(@",
                {0} = @{1}", coluna.nome, coluna.nome));
            }

            return colunasUpdate.ToString();
        }

        private string MontarParametrosNaoObrigatoriosInsertDAL()
        {
            StringBuilder result = new StringBuilder(@"
");
            IEnumerable<Coluna> auxLista = this.colunas.Where(coluna =>
                null != coluna &&
                !string.IsNullOrEmpty(coluna.nome) &&
                !coluna.autoIncremento &&
                !coluna.campoObrigatorio);
            if (auxLista != null && auxLista.Count() > 0)
            {
                foreach (Coluna coluna in auxLista)
                {
                    result.Append(@"
                object ").Append(coluna.nome).Append(@" = DBNull.Value;
                if (null != obj.").Append(coluna.nome).Append(@")
                {
                    ").Append(coluna.nome).Append(@" = obj.").Append(coluna.nome).Append(@";
                }
");
                }
            }
            return result.ToString();
        }

        private string MontarParametrosNaoObrigatoriosUpdateDAL()
        {
            StringBuilder result = new StringBuilder(@"
");
            IEnumerable<Coluna> auxLista = this.colunas.Where(coluna =>
                null != coluna &&
                !string.IsNullOrEmpty(coluna.nome) &&
                !coluna.autoIncremento &&
                !INDICE_PK.Equals(coluna.indice) &&
                !coluna.campoObrigatorio);
            if (auxLista != null && auxLista.Count() > 0)
            {
                foreach (Coluna coluna in auxLista)
                {
                    result.Append(@"
                object ").Append(coluna.nome).Append(@" = DBNull.Value;
                if (null != obj.").Append(coluna.nome).Append(@")
                {
                    ").Append(coluna.nome).Append(@" = obj.").Append(coluna.nome).Append(@";
                }
");
                }
            }
            return result.ToString();
        }

        private string ObterParametrosInsertDAL()
        {
            StringBuilder result = new StringBuilder();
            foreach (Coluna coluna in this.colunas.Where(coluna =>
                null != coluna &&
                !string.IsNullOrEmpty(coluna.nome) &&
                !coluna.autoIncremento))
            {
                result.Append(@"
                comm.Parameters.Add(new SqlParameter(").Append("\"").Append(coluna.nome).Append("\"").Append(coluna.campoObrigatorio ? ", obj." : ", ").Append(coluna.nome).Append("));");
            }
            return result.ToString();
        }

        private string ObterParametrosUpdateDAL()
        {
            StringBuilder result = new StringBuilder();
            foreach (Coluna coluna in this.colunas.Where(coluna =>
                null != coluna &&
                !string.IsNullOrEmpty(coluna.nome) &&
                !coluna.autoIncremento &&
                !INDICE_PK.Equals(coluna.indice)))
            {
                result.Append(@"
                comm.Parameters.Add(new SqlParameter(").Append("\"").Append(coluna.nome).Append("\"").Append(coluna.campoObrigatorio ? ", obj." : ", ").Append(coluna.nome).Append("));");
            }

            foreach (Coluna coluna in this.colunas.Where(coluna =>
                null != coluna &&
                !string.IsNullOrEmpty(coluna.nome) &&
                INDICE_PK.Equals(coluna.indice)))
            {
                result.Append(@"
                comm.Parameters.Add(new SqlParameter(").Append("\"").Append(coluna.nome).Append("\"").Append(", obj.").Append(coluna.nome).Append("));");
            }
            return result.ToString();
        }

        private string ObterParametrosDeleteDAL()
        {
            StringBuilder result = new StringBuilder();
            foreach (Coluna coluna in this.colunas.Where(coluna =>
                null != coluna &&
                !string.IsNullOrEmpty(coluna.nome) &&
                INDICE_PK.Equals(coluna.indice)))
            {
                result.Append(@"
                comm.Parameters.Add(new SqlParameter(").Append("\"").Append(coluna.nome).Append("\"").Append(", obj.").Append(coluna.nome).Append("));");
            }
            return result.ToString();
        }

        private string ObterWhereUpdateDeleteDAL()
        {
            StringBuilder chavesColunaUpdate = new StringBuilder();
            foreach (Coluna coluna in this.colunas.Where(coluna =>
                null != coluna &&
                !string.IsNullOrEmpty(coluna.nome) &&
                INDICE_PK.Equals(coluna.indice)))
            {
                if (string.IsNullOrEmpty(chavesColunaUpdate.ToString()))
                {
                    chavesColunaUpdate.Append(string.Format("WHERE {0} = @{1}", coluna.nome, coluna.nome));
                    continue;
                }
                chavesColunaUpdate.Append(string.Format(@"
                AND {0} = @{1}", coluna.nome, coluna.nome));
            }
            return chavesColunaUpdate.ToString();
        }

        private string ObterChaveGetDAL()
        {
            StringBuilder chaveGet = new StringBuilder();
            foreach (Coluna coluna in this.colunas.Where(coluna =>
                null != coluna &&
                !string.IsNullOrEmpty(coluna.nome) &&
                INDICE_PK.Equals(coluna.indice)))
            {
                chaveGet.Append(coluna.nome);
                break;
            }
            return chaveGet.ToString();
        }

        private string ObterSelectGetDAL()
        {
            StringBuilder selectGet = new StringBuilder();
            foreach (Coluna coluna in this.colunas.Where(coluna =>
                null != coluna &&
                !string.IsNullOrEmpty(coluna.nome)))
            {
                selectGet.Append(@"
                    ").Append(string.Format("{0},", coluna.nome));
            }
            return selectGet.ToString();
        }

        private string ObterColunaFiltroGetDAL(Coluna coluna)
        {
            if (coluna.tipoTexto)
            {
                return " collate Latin1_General_CI_AI like @textoFiltro";
            }
            return " like @textoFiltro";
        }

        private string ObterFiltroGetDAL()
        {
            StringBuilder filtroGet = new StringBuilder();
            foreach (Coluna coluna in this.colunas.Where(coluna =>
                null != coluna &&
                !string.IsNullOrEmpty(coluna.nome)))
            {
                if (string.IsNullOrEmpty(filtroGet.ToString()))
                {
                    filtroGet.Append("WHERE");
                    filtroGet.Append(@"
                        ").Append(coluna.nome).Append(this.ObterColunaFiltroGetDAL(coluna));
                    continue;
                }
                filtroGet.Append(@"
                        OR
                        ").Append(coluna.nome).Append(this.ObterColunaFiltroGetDAL(coluna));
            }
            return filtroGet.ToString();
        }

        private string ObterObjetoGetDAL()
        {
            StringBuilder objetoGet = new StringBuilder();
            IList<Coluna> auxColunas = this.colunas.Where(auxColuna =>
                null != auxColuna &&
                !string.IsNullOrEmpty(auxColuna.nome)).ToList<Coluna>();
            Coluna coluna;
            int i;
            for (i = 0; i < auxColunas.Count - 1; i++)
            {
                coluna = auxColunas[i];
                objetoGet.Append(@"
                        ").Append(string.Format("{0},", coluna.MontarCampoColuna(i)));
            }
            i = auxColunas.Count - 1;
            coluna = auxColunas[i];
            objetoGet.Append(@"
                        ").Append(coluna.MontarCampoColuna(i));
            return objetoGet.ToString();
        }

        private void MontarRegistroSelectChaveEstrangeira(ref StringBuilder chaveRegistro, ref StringBuilder valorRegistro)
        {
            IList<Coluna> auxColunas = this.colunas.Where(auxColuna =>
                null != auxColuna &&
                !string.IsNullOrEmpty(auxColuna.nome) &&
                INDICE_PK.Equals(auxColuna.indice)).ToList<Coluna>();
            if (auxColunas != null && auxColunas.Count > 0)
            {
                Coluna coluna;
                int i = 0;
                coluna = auxColunas[i];
                chaveRegistro.Append(coluna.nome);
                valorRegistro.Append(coluna.nome);
                for (i = 1; i < auxColunas.Count; i++)
                {
                    coluna = auxColunas[i];
                    chaveRegistro.Append(string.Format(" + ';' + {0}", coluna.nome));
                    valorRegistro.Append(string.Format(" + ' - ' + {0}", coluna.nome));
                }
            }
        }

        private string MontarMetodoGetDAL()
        {
            StringBuilder textoMetodoGet = new StringBuilder(@"
        public static IList<#NOME_TABELA_TO#> Get(int start, int pageSize, ref int totRegistros, string textoFiltro, ref int totRegistrosFiltro, string sortColumn, string sortColumnDir)
        {
            IList<#NOME_TABELA_TO#> objs = new List<#NOME_TABELA_TO#>();

            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                string ordenacao;
                if (string.IsNullOrEmpty(sortColumn))
                {
                    ordenacao = ").Append("\"ORDER BY ").Append(this.ObterChaveGetDAL()).Append("\";").Append(@"
                }
                else
                {
                    ordenacao = string.Format(").Append("\"ORDER BY {0} {1}\", sortColumn, sortColumnDir);").Append(@"
                }");
            textoMetodoGet.Append(@"
                StringBuilder queryGet = new StringBuilder(@").Append("\"");
            textoMetodoGet.Append(@"
                SELECT TOP (@pageSize) *
                FROM (
				    SELECT ").Append(this.ObterSelectGetDAL()).Append(@"

                    (ROW_NUMBER() OVER (").Append("\").Append(ordenacao).Append(@\"))").Append(@"
                    AS 'numeroLinha', 

                    (SELECT COUNT(").Append(this.ObterChaveGetDAL()).Append(@") FROM #NOME_TABELA#) 
				    AS 'totRegistros', 

					(SELECT COUNT(").Append(this.ObterChaveGetDAL()).Append(@") FROM #NOME_TABELA# 
					    ").Append(this.ObterFiltroGetDAL()).Append(@"
                    ) 
					AS 'totRegistrosFiltro'

	                FROM #NOME_TABELA#
						").Append(this.ObterFiltroGetDAL()).Append(@") 

				AS todasLinhas
                WHERE todasLinhas.numeroLinha > (@start)").Append("\"); ");
            textoMetodoGet.Append(@"

                comm.Parameters.Add(new SqlParameter(").Append("\"pageSize\"").Append(", pageSize));");
            textoMetodoGet.Append(@"
                comm.Parameters.Add(new SqlParameter(").Append("\"start\"").Append(", start));");
            textoMetodoGet.Append(@"
                comm.Parameters.Add(new SqlParameter(").Append("\"textoFiltro\"").Append(", string.Format(\"%{0}%\", textoFiltro)));");
            textoMetodoGet.Append(@"

                comm.CommandText = queryGet.ToString();

                con.Open();

                SqlDataReader rd = comm.ExecuteReader();

                #NOME_TABELA_TO# obj;

                if (rd.Read())
                {
                    totRegistros = rd.GetInt32(").Append(this.colunas.Count + 1).Append(@");
                    totRegistrosFiltro = rd.GetInt32(").Append(this.colunas.Count + 2).Append(@");

                    obj = new #NOME_TABELA_TO#
                    {").Append(this.ObterObjetoGetDAL()).Append(@"
                    };
                    objs.Add(obj);
                }
                while (rd.Read())
                {
                    obj = new #NOME_TABELA_TO#
                    {").Append(this.ObterObjetoGetDAL()).Append(@"
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
        ");

            return textoMetodoGet.ToString();
        }

        private string MontarMetodoGetParaChaveEstrangeiraDAL()
        {
            StringBuilder textoMetodoGet = new StringBuilder(@"
        public static IList<object> GetParaChaveEstrangeira()
        {
            IList<object> objs = new List<object>();

            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                StringBuilder queryGet = new StringBuilder(@").Append("\"");
            textoMetodoGet.Append(@"
                SELECT
                chaveRegistro = CONVERT(VARCHAR, #CHAVE_REGISTRO#),
                valorRegistro = CONVERT(VARCHAR, #VALOR_REGISTRO#)

                FROM #NOME_TABELA#

                ORDER BY valorRegistro").Append("\");");
            textoMetodoGet.Append(@"

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
        ");

            StringBuilder chaveRegistro = new StringBuilder();
            StringBuilder valorRegistro = new StringBuilder();

            this.MontarRegistroSelectChaveEstrangeira(ref chaveRegistro, ref valorRegistro);

            textoMetodoGet = textoMetodoGet.Replace("#CHAVE_REGISTRO#", chaveRegistro.ToString());
            textoMetodoGet = textoMetodoGet.Replace("#VALOR_REGISTRO#", valorRegistro.ToString());

            return textoMetodoGet.ToString();
        }

        private string MontarMetodoInsertDAL()
        {
            StringBuilder textoInsertSql = new StringBuilder("@\"");
            textoInsertSql.Append(@"
                INSERT INTO #NOME_TABELA# 
                (").Append(this.ObterColunasInsertDAL(false)).Append(@") VALUES 
                (").Append(this.ObterColunasInsertDAL(true)).Append(@")
                ");
            textoInsertSql.Append("\";");

            StringBuilder textoMetodoInsert = new StringBuilder(@"
        public static int? Insert(#NOME_TABELA_TO# obj)
        {
            int? nrLinhas;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;
            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;
                comm.CommandText = ");
            textoMetodoInsert.Append(textoInsertSql);
            textoMetodoInsert.Append(@"

                con.Open();").Append(this.MontarParametrosNaoObrigatoriosInsertDAL());
            textoMetodoInsert.Append(ObterParametrosInsertDAL().ToString());
            textoMetodoInsert.Append(@"

                nrLinhas = comm.ExecuteNonQuery();");
            textoMetodoInsert.Append(@"
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
                ");
            return textoMetodoInsert.ToString();
        }

        private string MontarMetodoUptadeDAL()
        {
            StringBuilder textoUpdateSql = new StringBuilder("@\"");
            textoUpdateSql.Append(@"
                UPDATE #NOME_TABELA# 
                ").Append(this.ObterColunasUpdateDAL()).Append(@"
                ").Append(this.ObterWhereUpdateDeleteDAL()).Append(@"
                ");
            textoUpdateSql.Append("\";");

            StringBuilder textoMetodoUpdate = new StringBuilder(@"
        public static int? Update(#NOME_TABELA_TO# obj)
        {
            int? nrLinhas;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;
            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;
                comm.CommandText = ");
            textoMetodoUpdate.Append(textoUpdateSql);
            textoMetodoUpdate.Append(@"

                con.Open();").Append(this.MontarParametrosNaoObrigatoriosInsertDAL());
            textoMetodoUpdate.Append(ObterParametrosUpdateDAL().ToString());
            textoMetodoUpdate.Append(@"

                nrLinhas = comm.ExecuteNonQuery();");
            textoMetodoUpdate.Append(@"
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
                ");
            return textoMetodoUpdate.ToString();
        }

        private string MontarMetodoDeleteDAL()
        {
            StringBuilder textoDeleteSql = new StringBuilder("@\"");
            textoDeleteSql.Append(@"
                DELETE #NOME_TABELA# 
                ").Append(this.ObterWhereUpdateDeleteDAL()).Append(@"
                ");
            textoDeleteSql.Append("\";");

            StringBuilder textoMetodoDelete = new StringBuilder(@"
        public static int? Delete(#NOME_TABELA_TO# obj)
        {
            int? nrLinhas;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;
            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;
                comm.CommandText = ");
            textoMetodoDelete.Append(textoDeleteSql);
            textoMetodoDelete.Append(@"

                con.Open();
        ");
            textoMetodoDelete.Append(ObterParametrosDeleteDAL().ToString());
            textoMetodoDelete.Append(@"

                nrLinhas = comm.ExecuteNonQuery();");
            textoMetodoDelete.Append(@"
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
        }");
            return textoMetodoDelete.ToString();
        }
        #endregion

        #region Controller
        private string ObterParametrosInsert()
        {
            StringBuilder result = new StringBuilder();
            foreach (Coluna coluna in this.colunas.Where(coluna =>
                null != coluna &&
                !string.IsNullOrEmpty(coluna.nome) &&
                !coluna.autoIncremento))
            {
                if (string.IsNullOrEmpty(result.ToString()))
                {
                    result.Append(coluna.ColunaParametroController());
                    continue;
                }
                result.Append(string.Format(", {0}", coluna.ColunaParametroController()));
            }
            return result.ToString();
        }

        private string MontarNomeColunaParaAux(string nomeColuna)
        {
            if (!string.IsNullOrEmpty(nomeColuna))
            {
                string.Format("{0}{1}", nomeColuna[0].ToString().ToUpper(), nomeColuna.Substring(1));
            }
            return nomeColuna;
        }

        private string ObterParametrosUpdate(bool adicionarTipo, bool javaScript)
        {
            StringBuilder result = new StringBuilder();
            if (javaScript)
            {
                IList<Coluna> auxColunas = this.colunas.Where(auxColuna =>
                null != auxColuna &&
                !string.IsNullOrEmpty(auxColuna.nome) &&
                !auxColuna.autoIncremento &&
                !INDICE_PK.Equals(auxColuna.indice)).ToList<Coluna>();
                Coluna coluna;
                int i;
                for (i = 0; i < auxColunas.Count - 1; i++)
                {
                    coluna = auxColunas[i];
                    if (coluna.campoObrigatorio)
                    {
                        result.Append("\\'' + objetoTabela.").Append(coluna.nome).Append(" + '\\', ");
                    }
                    else
                    {
                        result.Append("\\'' + aux").Append(this.MontarNomeColunaParaAux(coluna.nome)).Append(" + '\\', ");
                    }
                }

                IList<Coluna> auxColunasChave = this.colunas.Where(auxColuna =>
                    null != auxColuna &&
                    !string.IsNullOrEmpty(auxColuna.nome) &&
                    INDICE_PK.Equals(auxColuna.indice)).ToList<Coluna>();

                if (auxColunas != null && auxColunas.Count > 0)
                {
                    i = auxColunas.Count - 1;
                    coluna = auxColunas[i];

                    if (auxColunasChave != null && auxColunasChave.Count > 0)
                    {
                        if (coluna.campoObrigatorio)
                        {
                            result.Append("\\'' + objetoTabela.").Append(coluna.nome).Append(" + '\\', ");
                        }
                        else
                        {
                            result.Append("\\'' + aux").Append(this.MontarNomeColunaParaAux(coluna.nome)).Append(" + '\\', ");
                        }
                    }
                    else
                    {
                        if (coluna.campoObrigatorio)
                        {
                            result.Append("\\'' + objetoTabela.").Append(coluna.nome).Append(" + '\\'");
                        }
                        else
                        {
                            result.Append("\\'' + aux").Append(this.MontarNomeColunaParaAux(coluna.nome)).Append(" + '\\'");
                        }
                    }
                }

                for (i = 0; i < auxColunasChave.Count - 1; i++)
                {
                    coluna = auxColunasChave[i];
                    result.Append("\\'' + objetoTabela.").Append(coluna.nome).Append(" + '\\', ");
                }

                i = auxColunasChave.Count - 1;
                coluna = auxColunasChave[i];
                result.Append("\\'' + objetoTabela.").Append(coluna.nome).Append(" + '\\'");
            }
            else
            {
                foreach (Coluna coluna in this.colunas.Where(coluna =>
                null != coluna &&
                !string.IsNullOrEmpty(coluna.nome) &&
                !coluna.autoIncremento &&
                !INDICE_PK.Equals(coluna.indice)))
                {
                    if (string.IsNullOrEmpty(result.ToString()))
                    {
                        result.Append(adicionarTipo ? coluna.ColunaParametroController() : coluna.nome);
                        continue;
                    }
                    result.Append(string.Format(", {0}", adicionarTipo ? coluna.ColunaParametroController() : coluna.nome));
                }

                foreach (Coluna coluna in this.colunas.Where(coluna =>
                    null != coluna &&
                    !string.IsNullOrEmpty(coluna.nome) &&
                    INDICE_PK.Equals(coluna.indice)))
                {
                    if (string.IsNullOrEmpty(result.ToString()))
                    {
                        result.Append(adicionarTipo ? coluna.ColunaParametroController() : coluna.nome);
                        continue;
                    }
                    result.Append(string.Format(", {0}", adicionarTipo ? coluna.ColunaParametroController() : coluna.nome));
                }
            }
            return result.ToString();
        }

        private string ObterParametrosDelete()
        {
            StringBuilder result = new StringBuilder();
            foreach (Coluna coluna in this.colunas.Where(coluna =>
                null != coluna &&
                !string.IsNullOrEmpty(coluna.nome) &&
                INDICE_PK.Equals(coluna.indice)))
            {
                if (string.IsNullOrEmpty(result.ToString()))
                {
                    result.Append(coluna.ColunaParametroController());
                    continue;
                }
                result.Append(string.Format(", {0}", coluna.ColunaParametroController()));
            }
            return result.ToString();
        }

        private string MontarObjetoInsert(bool javaScript)
        {
            StringBuilder result = new StringBuilder();
            if (javaScript)
            {
                StringBuilder camposChavesEstrangeiras = new StringBuilder();
                ISet<String> chavesEstrangeirasAdicionadas = new HashSet<String>();
                ChaveEstrangeira chaveEstrangeira;
                IList<Coluna> auxColunas = this.colunas.Where(auxColuna =>
                null != auxColuna &&
                !string.IsNullOrEmpty(auxColuna.nome) &&
                !auxColuna.autoIncremento).ToList<Coluna>();
                Coluna coluna;
                int i;
                for (i = 0; i < auxColunas.Count - 1; i++)
                {
                    coluna = auxColunas[i];
                    chaveEstrangeira = this.ObterchaveEstrangeira(coluna.nome);
                    if (chaveEstrangeira != null)
                    {
                        if (!chavesEstrangeirasAdicionadas.Contains(chaveEstrangeira.nomeTabelaReferenciada))
                        {
                            camposChavesEstrangeiras.Append(chaveEstrangeira.MontarParaDadosCadastro(!string.IsNullOrEmpty(camposChavesEstrangeiras.ToString())));
                            chavesEstrangeirasAdicionadas.Add(chaveEstrangeira.nomeTabelaReferenciada);
                        }
                    }
                    else
                    {
                        result.Append(@"
        ").Append(string.Format("{0}: $('#{1}').val(),", coluna.nome, coluna.nome));
                    }
                }
                bool adicionarVirgula = false;
                i = auxColunas.Count - 1;
                coluna = auxColunas[i];
                chaveEstrangeira = this.ObterchaveEstrangeira(coluna.nome);
                if (chaveEstrangeira != null)
                {
                    if (!chavesEstrangeirasAdicionadas.Contains(chaveEstrangeira.nomeTabelaReferenciada))
                    {
                        camposChavesEstrangeiras.Append(chaveEstrangeira.MontarParaDadosCadastro(!string.IsNullOrEmpty(camposChavesEstrangeiras.ToString())));
                        chavesEstrangeirasAdicionadas.Add(chaveEstrangeira.nomeTabelaReferenciada);
                    }
                }
                else
                {
                    result.Append(@"
        ").Append(string.Format("{0}: $('#{1}').val()", coluna.nome, coluna.nome));
                    adicionarVirgula = true;
                }
                if (!string.IsNullOrEmpty(camposChavesEstrangeiras.ToString()))
                {
                    if (!string.IsNullOrEmpty(result.ToString()) && adicionarVirgula)
                    {
                        result.Append(",");
                    }
                    result.Append(camposChavesEstrangeiras.ToString());
                }
            }
            else
            {
                IList<Coluna> auxColunas = this.colunas.Where(auxColuna =>
                null != auxColuna &&
                !string.IsNullOrEmpty(auxColuna.nome) &&
                !auxColuna.autoIncremento).ToList<Coluna>();
                Coluna coluna;
                int i;
                for (i = 0; i < auxColunas.Count - 1; i++)
                {
                    coluna = auxColunas[i];
                    result.Append(@"
                ").Append(string.Format("{0} = {1},", coluna.nome, coluna.nome));
                }
                i = auxColunas.Count - 1;
                coluna = auxColunas[i];
                result.Append(@"
                ").Append(string.Format("{0} = {1}", coluna.nome, coluna.nome));
            }
            return result.ToString();
        }

        private string MontarObjetoUpdate()
        {
            StringBuilder result = new StringBuilder();
            IList<Coluna> auxColunas = this.colunas.Where(auxColuna =>
                null != auxColuna &&
                !string.IsNullOrEmpty(auxColuna.nome) &&
                !auxColuna.autoIncremento &&
                !INDICE_PK.Equals(auxColuna.indice)).ToList<Coluna>();
            Coluna coluna;
            int i;
            for (i = 0; i < auxColunas.Count - 1; i++)
            {
                coluna = auxColunas[i];
                result.Append(@"
                ").Append(string.Format("{0} = {1},", coluna.nome, coluna.nome));
            }

            IList<Coluna> auxColunasChave = this.colunas.Where(auxColuna =>
                null != auxColuna &&
                !string.IsNullOrEmpty(auxColuna.nome) &&
                INDICE_PK.Equals(auxColuna.indice)).ToList<Coluna>();

            if (auxColunas != null && auxColunas.Count > 0)
            {
                i = auxColunas.Count - 1;
                coluna = auxColunas[i];

                if (auxColunasChave != null && auxColunasChave.Count > 0)
                {
                    result.Append(@"
                ").Append(string.Format("{0} = {1},", coluna.nome, coluna.nome));
                }
                else
                {
                    result.Append(@"
                ").Append(string.Format("{0} = {1}", coluna.nome, coluna.nome));
                }
            }

            for (i = 0; i < auxColunasChave.Count - 1; i++)
            {
                coluna = auxColunasChave[i];
                result.Append(@"
                ").Append(string.Format("{0} = {1},", coluna.nome, coluna.nome));
            }

            i = auxColunasChave.Count - 1;
            coluna = auxColunasChave[i];
            result.Append(@"
                ").Append(string.Format("{0} = {1}", coluna.nome, coluna.nome));

            return result.ToString();
        }

        private string MontarObjetoDelete()
        {
            StringBuilder result = new StringBuilder();
            IList<Coluna> auxColunas = this.colunas.Where(auxColuna =>
                null != auxColuna &&
                !string.IsNullOrEmpty(auxColuna.nome) &&
                INDICE_PK.Equals(auxColuna.indice)).ToList<Coluna>();
            Coluna coluna;
            int i;
            for (i = 0; i < auxColunas.Count - 1; i++)
            {
                coluna = auxColunas[i];
                result.Append(@"
                ").Append(string.Format("{0} = {1},", coluna.nome, coluna.nome));
            }
            i = auxColunas.Count - 1;
            coluna = auxColunas[i];
            result.Append(@"
                ").Append(string.Format("{0} = {1}", coluna.nome, coluna.nome));
            return result.ToString();
        }

        private string MontarMetodoIndex()
        {
            return @"
        public ActionResult Index() 
        {
            return View();
        }
            ";
        }

        private string MontarMetodoGet()
        {
            return new StringBuilder(@"
        [HttpPost]
        public JsonResult Get()
        {
            int draw = Convert.ToInt32(Request.Form[").Append("\"draw\"").Append(@"]);
            int start = Convert.ToInt32(Request.Form[").Append("\"start\"").Append(@"]);
            int length = Convert.ToInt32(Request.Form[").Append("\"length\"").Append(@"]);
            string textoFiltro = Request.Form[").Append("\"search[value]\"").Append(@"];
            string sortColumn = Request.Form[string.Format(").Append("\"columns[{0}][name]\"").Append(", Request.Form[").Append("\"order[0][column]\"").Append(@"])];
            string sortColumnDir = Request.Form[").Append("\"order[0][dir]\"").Append(@"];

            int totRegistros = 0;
            int totRegistrosFiltro = 0;
            IList<#NOME_TABELA_TO#> dados = #NOME_CLASSE_DAL#.Get(start, length, ref totRegistros, textoFiltro, ref totRegistrosFiltro, sortColumn, sortColumnDir);
            if (start > 0 && dados.Count == 0)
            {
                start -= length;
                dados = #NOME_CLASSE_DAL#.Get(start, length, ref totRegistros, textoFiltro, ref totRegistrosFiltro, sortColumn, sortColumnDir);
                return Json(new { draw = draw, recordsFiltered = totRegistrosFiltro, recordsTotal = totRegistros, data = dados, voltarPagina = 'S' }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { draw = draw, recordsFiltered = totRegistrosFiltro, recordsTotal = totRegistros, data = dados }, JsonRequestBehavior.AllowGet);
        }
            ").ToString();
        }

        private string MontarMetodoGetParaChaveEstrangeira()
        {
            return new StringBuilder(@"
        [HttpPost]
        public JsonResult GetParaChaveEstrangeira()
        {
            IList<object> dados = #NOME_CLASSE_DAL#.GetParaChaveEstrangeira();
            return Json(new { data = dados }, JsonRequestBehavior.AllowGet);
        }
            ").ToString();
        }

        private string MontarMetodoInsert()
        {
            return new StringBuilder(@"
        [HttpPost]
        public JsonResult Insert(").Append(this.ObterParametrosInsert()).Append(@")
        {
            string auxMsgErro = string.Empty;
            string auxMsgSucesso = string.Empty;

            #NOME_TABELA_TO# obj = new #NOME_TABELA_TO#
            {").Append(this.MontarObjetoInsert(false)).Append(@"
            };

            if (#NOME_CLASSE_DAL#.Insert(obj) == null)
            {
                auxMsgErro = ").Append("\"Falha ao tentar inserir o registro, favor tente novamente\"").Append(@";
            }
            else
            {
                auxMsgSucesso = ").Append("\"Registro inserido com sucesso\"").Append(@";
            }

            return Json(new { msgErro = auxMsgErro, msgSucesso = auxMsgSucesso });
        }
            ").ToString();
        }

        private string MontarMetodoUpdate()
        {
            return new StringBuilder(@"
        [HttpPost]
        public JsonResult Update(").Append(this.ObterParametrosUpdate(true, false)).Append(@")
        {
            string auxMsgErro = string.Empty;
            string auxMsgSucesso = string.Empty;

            #NOME_TABELA_TO# obj = new #NOME_TABELA_TO#
            {").Append(this.MontarObjetoUpdate()).Append(@"
            };

            if (#NOME_CLASSE_DAL#.Update(obj) == null)
            {
                auxMsgErro = ").Append("\"Falha ao tentar alterar o registro, favor tente novamente\"").Append(@";
            }
            else
            {
                auxMsgSucesso = ").Append("\"Registro alterado com sucesso\"").Append(@";
            }

            return Json(new { msgErro = auxMsgErro, msgSucesso = auxMsgSucesso });
        }
            ").ToString();
        }

        private string MontarMetodoDelete()
        {
            return new StringBuilder(@"
        [HttpPost]
        public JsonResult Delete(").Append(this.ObterParametrosDelete()).Append(@")
        {
            string auxMsgErro = string.Empty;
            string auxMsgSucesso = string.Empty;

            #NOME_TABELA_TO# obj = new #NOME_TABELA_TO#
            {").Append(this.MontarObjetoDelete()).Append(@"
            };

            if (#NOME_CLASSE_DAL#.Delete(obj) == null)
            {
                auxMsgErro = ").Append("\"Falha ao tentar excluir o registro, favor tente novamente\"").Append(@";
            }
            else
            {
                auxMsgSucesso = ").Append("\"Registro excluído com sucesso\"").Append(@";
            }

            return Json(new { msgErro = auxMsgErro, msgSucesso = auxMsgSucesso });
        }").ToString();
        }
        #endregion

        #region JavaScript
        private string ObterCamposFormCadastroJavaScript()
        {
            StringBuilder camposFormCadastro = new StringBuilder();
            ISet<String> chavesEstrangeirasAdicionadas = new HashSet<String>();
            ChaveEstrangeira chaveEstrangeira;
            IList<Coluna> auxColunas = this.colunas.Where(coluna =>
                    null != coluna &&
                    !string.IsNullOrEmpty(coluna.nome) &&
                    !coluna.autoIncremento &&
                    coluna.campoObrigatorio).ToList<Coluna>();
            if (null != auxColunas && auxColunas.Count > 0)
            {
                Coluna coluna;
                int i = 0;
                coluna = auxColunas[i];
                chaveEstrangeira = this.ObterchaveEstrangeira(coluna.nome);
                if (chaveEstrangeira != null)
                {
                    if (!chavesEstrangeirasAdicionadas.Contains(chaveEstrangeira.nomeTabelaReferenciada))
                    {
                        camposFormCadastro.Append(@"
    ").Append(chaveEstrangeira.nomeTabelaReferenciada).Append(@": {
        validators: {
            notEmpty: {
                message: 'Campo obrigat&oacute;rio'
            }
        }
    }");
                        chavesEstrangeirasAdicionadas.Add(chaveEstrangeira.nomeTabelaReferenciada);
                    }
                }
                else
                {
                    camposFormCadastro.Append(@"
    ").Append(coluna.nome).Append(@": {
        validators: {
            notEmpty: {
                message: 'Campo obrigat&oacute;rio'
            }
        }
    }");
                }
                for (i = 1; i < auxColunas.Count; i++)
                {
                    coluna = auxColunas[i];
                    chaveEstrangeira = this.ObterchaveEstrangeira(coluna.nome);
                    if (chaveEstrangeira != null)
                    {
                        if (!chavesEstrangeirasAdicionadas.Contains(chaveEstrangeira.nomeTabelaReferenciada))
                        {
                            camposFormCadastro.Append(@",
    ").Append(chaveEstrangeira.nomeTabelaReferenciada).Append(@": {
        validators: {
            notEmpty: {
                message: 'Campo obrigat&oacute;rio'
            }
        }
    }");
                            chavesEstrangeirasAdicionadas.Add(chaveEstrangeira.nomeTabelaReferenciada);
                        }
                    }
                    else
                    {
                        camposFormCadastro.Append(@",
    ").Append(coluna.nome).Append(@": {
        validators: {
            notEmpty: {
                message: 'Campo obrigat&oacute;rio'
            }
        }
    }");
                    }
                }
            }
            return camposFormCadastro.ToString();
        }

        private string ObterColunasTabelaPrincipalJavaScript()
        {
            StringBuilder colunasTabelaPrincipal = new StringBuilder();
            IList<Coluna> auxColunas = this.colunas.Where(coluna =>
                    null != coluna &&
                    !string.IsNullOrEmpty(coluna.nome)).ToList<Coluna>();
            if (null != auxColunas && auxColunas.Count > 0)
            {
                Coluna coluna;
                int i = 0;
                coluna = auxColunas[i];
                colunasTabelaPrincipal.Append(@"
    {
        data: '").Append(coluna.nome).Append(@"',
        name: '").Append(coluna.nome).Append(@"'
    }");
                for (i = 1; i < auxColunas.Count - 1; i++)
                {
                    coluna = auxColunas[i];
                    colunasTabelaPrincipal.Append(@",
    {
        data: '").Append(coluna.nome).Append(@"',
        name: '").Append(coluna.nome).Append(@"'
    }");
                }
                i = auxColunas.Count - 1;
                coluna = auxColunas[i];
                colunasTabelaPrincipal.Append(@",
    {
        data: '").Append(coluna.nome).Append(@"',
        name: '").Append(coluna.nome).Append(@"'
    }");
            }
            return colunasTabelaPrincipal.ToString();
        }

        private string ObterCamposPreenchimentoJavaScript(bool setarValor, ref bool adicionarListaSelects)
        {
            StringBuilder camposPreenchimento = new StringBuilder();
            StringBuilder listaChecagemRegistrosObtidos = new StringBuilder();
            StringBuilder listaSelects = new StringBuilder();
            ISet<String> chavesEstrangeirasAdicionadas = new HashSet<String>();
            ChaveEstrangeira chaveEstrangeira;
            foreach (Coluna coluna in this.colunas.Where(coluna =>
                     null != coluna &&
                     !string.IsNullOrEmpty(coluna.nome) &&
                     !coluna.autoIncremento))
            {
                chaveEstrangeira = this.ObterchaveEstrangeira(coluna.nome);
                if (chaveEstrangeira != null)
                {
                    if (!chavesEstrangeirasAdicionadas.Contains(chaveEstrangeira.nomeTabelaReferenciada))
                    {
                        if (coluna.campoObrigatorio)
                        {
                            listaChecagemRegistrosObtidos.Append(@"
    if (registrosObtidosPara").Append(chaveEstrangeira.nomeTabelaReferenciada).Append(@") {
        $('#").Append(chaveEstrangeira.nomeTabelaReferenciada).Append(@"')[0].selectize.setValue(").Append(setarValor ? chaveEstrangeira.MontarIdRegistroEscolhido() : "''").Append(@");
        $('#formCadastro').data('bootstrapValidator').updateStatus('").Append(chaveEstrangeira.nomeTabelaReferenciada).Append(@"', 'NOT_VALIDATED');
    }    
");
                        }
                        else
                        {
                            listaChecagemRegistrosObtidos.Append(@"
    if (registrosObtidosPara").Append(chaveEstrangeira.nomeTabelaReferenciada).Append(@") {
        $('#").Append(chaveEstrangeira.nomeTabelaReferenciada).Append(@"')[0].selectize.setValue(").Append(setarValor ? chaveEstrangeira.MontarIdRegistroEscolhido() : "''").Append(@");
    }    
");
                        }
                        if (chavesEstrangeirasAdicionadas.Count == 0)
                        {
                            listaSelects.Append(@"
    var listaSelects =
    [
        {");
                        }
                        else
                        {
                            listaSelects.Append(@",
        {");
                        }
                        listaSelects.Append(@"
            registrosObtidos: registrosObtidosPara").Append(chaveEstrangeira.nomeTabelaReferenciada).Append(@",
            idRegistroEscolhido: ").Append(setarValor ? chaveEstrangeira.MontarIdRegistroEscolhido() : "undefined").Append(@",
            funcaoInicializacao: inicializarRegistrosPara").Append(chaveEstrangeira.nomeTabelaReferenciada).Append(@"
        }");
                        chavesEstrangeirasAdicionadas.Add(chaveEstrangeira.nomeTabelaReferenciada);
                    }
                }
                else
                {
                    camposPreenchimento.Append(@"
    $('#").Append(coluna.nome).Append("').val(").Append(setarValor ? coluna.nome : "''").Append(");");
                }
            }
            if (chavesEstrangeirasAdicionadas.Count > 0)
            {
                listaSelects.Append(@"
    ];");
                adicionarListaSelects = true;
            }
            if (!string.IsNullOrEmpty(listaSelects.ToString()))
            {
                if (!string.IsNullOrEmpty(camposPreenchimento.ToString()))
                {
                    camposPreenchimento.Append(@"
").Append(listaChecagemRegistrosObtidos.ToString());
                    camposPreenchimento.Append(listaSelects.ToString());
                }
                else
                {
                    camposPreenchimento.Append(listaChecagemRegistrosObtidos.ToString());
                    camposPreenchimento.Append(listaSelects.ToString());
                }
            }
            return camposPreenchimento.ToString();
        }

        private string ObterCamposPreenchimentoJavaScriptSomenteLeitura()
        {
            StringBuilder camposPreenchimento = new StringBuilder();
            ISet<String> chavesEstrangeirasAdicionadas = new HashSet<String>();
            ChaveEstrangeira chaveEstrangeira;
            String nomeRegistroEscolhido;
            foreach (Coluna coluna in this.colunas.Where(coluna =>
                     null != coluna &&
                     !string.IsNullOrEmpty(coluna.nome) &&
                     !coluna.autoIncremento))
            {
                chaveEstrangeira = this.ObterchaveEstrangeira(coluna.nome);
                if (chaveEstrangeira != null)
                {
                    if (!chavesEstrangeirasAdicionadas.Contains(chaveEstrangeira.nomeTabelaReferenciada))
                    {
                        nomeRegistroEscolhido = chaveEstrangeira.MontarNomeRegistroEscolhido();
                        camposPreenchimento.Append(@"
    $('#").Append(string.Format("{0}Leitura", chaveEstrangeira.nomeTabelaReferenciada)).Append("').html(").Append(nomeRegistroEscolhido).Append(");");
                        chavesEstrangeirasAdicionadas.Add(chaveEstrangeira.nomeTabelaReferenciada);
                    }
                }
                else
                {
                    camposPreenchimento.Append(@"
    $('#").Append(string.Format("{0}Leitura", coluna.nome)).Append("').html(").Append(coluna.nome).Append(");");
                }
            }
            if (!string.IsNullOrEmpty(camposPreenchimento.ToString()))
            {
                return string.Format(@"
{0}", camposPreenchimento.ToString());
            }
            return string.Empty;
        }

        private string ObterIdRegistroJavaScript(bool exclusao, bool auxData)
        {
            StringBuilder idRegistro = new StringBuilder();
            IList<Coluna> auxColunas = this.colunas.Where(coluna =>
                null != coluna &&
                !string.IsNullOrEmpty(coluna.nome) &&
                INDICE_PK.Equals(coluna.indice)).ToList<Coluna>();
            if (null != auxColunas && auxColunas.Count > 0)
            {
                if (exclusao)
                {
                    Coluna coluna;
                    int i;
                    for (i = 0; i < auxColunas.Count - 1; i++)
                    {
                        coluna = auxColunas[i];
                        idRegistro.Append("\\'").Append(coluna.nome).Append("\\': \\'' + objetoTabela.").Append(coluna.nome).Append(" + '\\',");
                    }
                    i = auxColunas.Count - 1;
                    coluna = auxColunas[i];
                    idRegistro.Append("\\'").Append(coluna.nome).Append("\\': \\'' + objetoTabela.").Append(coluna.nome).Append(" + '\\'");
                }
                else if (auxData)
                {
                    Coluna coluna;
                    int i;
                    for (i = 0; i < auxColunas.Count; i++)
                    {
                        coluna = auxColunas[i];
                        idRegistro.Append(@"
            auxData.").Append(coluna.nome).Append(" = auxId.").Append(coluna.nome).Append(";");
                    }
                }
                else
                {
                    Coluna coluna;
                    int i;
                    for (i = 0; i < auxColunas.Count - 1; i++)
                    {
                        coluna = auxColunas[i];
                        idRegistro.Append(@"
        ").Append(coluna.nome).Append(": ").Append(coluna.nome).Append(",");
                    }
                    i = auxColunas.Count - 1;
                    coluna = auxColunas[i];
                    idRegistro.Append(@"
        ").Append(coluna.nome).Append(": ").Append(coluna.nome);
                }
            }
            return idRegistro.ToString();
        }

        private string MontarVariaveisControleChavesEstrangeiras()
        {
            StringBuilder result = new StringBuilder();
            if (this.chavesEstrangeiras != null && this.chavesEstrangeiras.Count > 0)
            {
                foreach (ChaveEstrangeira chaveEstrangeira in this.chavesEstrangeiras)
                {
                    result.Append(string.Format("var registrosObtidosPara{0} = false;", chaveEstrangeira.nomeTabelaReferenciada)).Append(@"
");
                }
            }
            return result.ToString();
        }

        private String MontarMetodosAjaxListaSelects()
        {
            StringBuilder result = new StringBuilder();
            ISet<String> chavesEstrangeirasAdicionadas = new HashSet<String>();
            ChaveEstrangeira chaveEstrangeira;
            foreach (Coluna coluna in this.colunas.Where(coluna =>
                null != coluna &&
                !string.IsNullOrEmpty(coluna.nome) &&
                !coluna.autoIncremento))
            {
                if (null != coluna &&
                !string.IsNullOrEmpty(coluna.nome))
                {
                    chaveEstrangeira = this.ObterchaveEstrangeira(coluna.nome);
                    if (chaveEstrangeira != null)
                    {
                        if (!chavesEstrangeirasAdicionadas.Contains(chaveEstrangeira.nomeTabelaReferenciada))
                        {
                            result.Append(@"
function inicializarRegistrosPara").Append(chaveEstrangeira.nomeTabelaReferenciada).Append(@"(chaveRegistro) {
    mostrarPopup();

    var auxOption = $('<option value="""">Informe o registro</option>');
    $('##ID_CAMPO_CHAVE_ESTRANGEIRA#').append(auxOption);

    $.ajax({
        url: '").Append(chaveEstrangeira.nomeTabelaReferenciada).Append(@"/GetParaChaveEstrangeira',
        type: 'POST',
        success: function (result) {
            var dados = result.data;
            for (var i = 0; i < dados.length; i++) {
                auxOption = $('<option value=""' + dados[i].chaveRegistro + '"">' + dados[i].valorRegistro + '</option>');
                $('##ID_CAMPO_CHAVE_ESTRANGEIRA#').append(auxOption);
            }
            $('##ID_CAMPO_CHAVE_ESTRANGEIRA#').selectize();

            if (undefined == chaveRegistro) {
                $('##ID_CAMPO_CHAVE_ESTRANGEIRA#')[0].selectize.setValue('');
            } else {
                $('##ID_CAMPO_CHAVE_ESTRANGEIRA#')[0].selectize.setValue(chaveRegistro);
            }
            ").Append(coluna.campoObrigatorio ? @"$('#formCadastro').data('bootstrapValidator').updateStatus('#ID_CAMPO_CHAVE_ESTRANGEIRA#', 'NOT_VALIDATED');
            " : string.Empty).Append(@"
            registrosObtidosPara").Append(chaveEstrangeira.nomeTabelaReferenciada).Append(@" = true;

            fecharPopup();
        },
        error: function (request, status, error) {
            $('##ID_CAMPO_CHAVE_ESTRANGEIRA#').selectize();
            mostrarMsgErro('Falha ao tentar obter os registros, favor tente novamente');
            fecharPopup();
        }
    });
}
");
                            result = result.Replace("#ID_CAMPO_CHAVE_ESTRANGEIRA#", chaveEstrangeira.nomeTabelaReferenciada);
                            chavesEstrangeirasAdicionadas.Add(chaveEstrangeira.nomeTabelaReferenciada);
                        }
                    }
                }
            }
            return result.ToString();
        }

        private String MontarAuxValorParametrosAlteracao()
        {
            StringBuilder result = new StringBuilder();
            foreach(Coluna coluna in this.colunas.Where(auxColuna =>
                null != auxColuna &&
                !string.IsNullOrEmpty(auxColuna.nome) &&
                !auxColuna.autoIncremento &&
                !INDICE_PK.Equals(auxColuna.indice) &&
                !auxColuna.campoObrigatorio))
            {
                result.Append(string.Format(@"
    var aux{0} = isNullOrEmpty(objetoTabela.{1}) ? '' : objetoTabela.{1};", this.MontarNomeColunaParaAux(coluna.nome), coluna.nome));
            }
            return result.ToString();
        }
        #endregion

        #region View
        private string MontarColunasGridView()
        {
            StringBuilder result = new StringBuilder();
            foreach (Coluna coluna in this.colunas)
            {
                if (null != coluna &&
                !string.IsNullOrEmpty(coluna.nome))
                {
                    result.Append(string.Format(@"
                                <th>{0}</th>", coluna.nome));
                }
            }
            return result.ToString();
        }

        private string MontarCamposFormulario()
        {
            StringBuilder result = new StringBuilder();
            int totColunasLinha = 0;
            result.Append(@"
                    <div class=""row"">");
            ISet<String> chavesEstrangeirasAdicionadas = new HashSet<String>();
            ChaveEstrangeira chaveEstrangeira;
            foreach (Coluna coluna in this.colunas.Where(coluna =>
                null != coluna &&
                !string.IsNullOrEmpty(coluna.nome) &&
                !coluna.autoIncremento))
            {
                if (null != coluna &&
                !string.IsNullOrEmpty(coluna.nome))
                {
                    if (totColunasLinha == 2)
                    {
                        result.Append(@"
                    </div>
                    <div class=""row"">");
                    }
                    chaveEstrangeira = this.ObterchaveEstrangeira(coluna.nome);
                    if (chaveEstrangeira != null)
                    {
                        if (!chavesEstrangeirasAdicionadas.Contains(chaveEstrangeira.nomeTabelaReferenciada))
                        {
                            result.Append(string.Format(@"
                        <div class=""col-lg-6 form-group campoCadastro"">
                            <label>{0}:</label>
                            <select id=""{0}"" name=""{0}"" class=""form-control""></select>
                        </div>", chaveEstrangeira.nomeTabelaReferenciada)).Append(string.Format(@"
                        <div class=""col-lg-6 form-group campoLeitura"">
                            <label>{0}:</label>
                            <p id=""{0}Leitura""></p>
                        </div>", chaveEstrangeira.nomeTabelaReferenciada));
                            chavesEstrangeirasAdicionadas.Add(chaveEstrangeira.nomeTabelaReferenciada);
                            totColunasLinha++;
                        }
                    }
                    else
                    {
                        result.Append(string.Format(@"
                        <div class=""col-lg-6 form-group campoCadastro"">
                            <label>{0}:</label>
                            <input id=""{0}"" name=""{0}"" class=""form-control"" placeholder=""Informe o {0}"" maxlength=""{1}""/>
                        </div>", coluna.nome, coluna.tamanho)).Append(string.Format(@"
                        <div class=""col-lg-6 form-group campoLeitura"">
                            <label>{0}:</label>
                            <p id=""{0}Leitura""></p>
                        </div>", coluna.nome));
                        totColunasLinha++;
                    }
                }
            }
            result.Append(@"
                    </div>");
            return result.ToString();
        }
        #endregion

        public Tabela(string nomeNameSpace, string nome, string pathTO, string pathDAL, string pathController, string pathView, string pathScript, Util util)
        {
            this.nomeNameSpace = nomeNameSpace;
            this.nome = nome;
            this.pathTO = pathTO;
            this.pathDAL = pathDAL;
            this.pathController = pathController;
            this.pathView = pathView;
            this.pathScript = pathScript;
            if (util != null)
            {
                this.util = util;
                this.colunas = this.util.GetColunasTabela(nome);
                this.chavesEstrangeiras = this.util.GetChavesEstrangeiras(nome);
                this.tabelaReferenciada = this.util.TabelaReferenciada(nome);
            }
        }

        public void CriarArquivoTO()
        {
            if (null != this.colunas && this.colunas.Count > 0)
            {
                String nomeArquivoTO = string.Format("{0}{1}.cs", this.nome, SIGLA_TO);
                String pathArquivoTO = string.Format("{0}\\{1}", this.pathTO, nomeArquivoTO);

                if (File.Exists(pathArquivoTO))
                {
                    File.Delete(pathArquivoTO);
                }

                StreamWriter sw = File.CreateText(pathArquivoTO);

                StringBuilder camposClasse = new StringBuilder();
                foreach (Coluna coluna in this.colunas)
                {
                    if (null != coluna && !string.IsNullOrEmpty(coluna.tipoEmCsharp))
                    {
                        camposClasse.Append(@"
		public #TIPO_COLUNA# #NOME_COLUNA# { get; set; }"
                        .Replace("#TIPO_COLUNA#", coluna.tipoEmCsharp)
                        .Replace("#NOME_COLUNA#", coluna.nome));
                    }
                }

                StringBuilder textoArquivo = new StringBuilder(@"using System;

namespace #NOME_NAMESPACE#.Models.#SIGLA_TO#
{
    public class #NOME_CLASSE#
    {");
                textoArquivo.Append(camposClasse.ToString());
                textoArquivo.Append(@"
    }
}");
                textoArquivo = textoArquivo.Replace("#NOME_NAMESPACE#", nomeNameSpace);
                textoArquivo = textoArquivo.Replace("#SIGLA_TO#", SIGLA_TO);
                textoArquivo = textoArquivo.Replace("#NOME_CLASSE#", string.Format("{0}{1}", this.nome, SIGLA_TO));
                textoArquivo = textoArquivo.Replace("#NOME_TABELA#", this.nome);

                sw.Write(textoArquivo);

                sw.Close();
            }
            else
            {
                MessageBox.Show(string.Format("Nenhuma coluna foi encontrada para a tabela {0}", this.nome));
            }
        }

        public void CriarArquivoDAL()
        {
            if (null != this.colunas && this.colunas.Count > 0)
            {
                String nomeArquivoDAL = string.Format("{0}{1}.cs", this.nome, SIGLA_DAL);
                String pathArquivoDAL = string.Format("{0}\\{1}", this.pathDAL, nomeArquivoDAL);

                if (File.Exists(pathArquivoDAL))
                {
                    File.Delete(pathArquivoDAL);
                }

                StreamWriter sw = File.CreateText(pathArquivoDAL);

                StringBuilder textoArquivo = new StringBuilder(@"using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using #NOME_NAMESPACE#.Models.TO;

namespace #NOME_NAMESPACE#.Models.#SIGLA_DAL#
{
    public class #NOME_CLASSE#
    {");
                textoArquivo.Append(this.MontarMetodoGetDAL());
                if (this.tabelaReferenciada)
                {
                    textoArquivo.Append(this.MontarMetodoGetParaChaveEstrangeiraDAL());
                }
                textoArquivo.Append(this.MontarMetodoInsertDAL());
                textoArquivo.Append(this.MontarMetodoUptadeDAL());
                textoArquivo.Append(this.MontarMetodoDeleteDAL()).Append(@"
    }
}");
                textoArquivo = textoArquivo.Replace("#NOME_NAMESPACE#", nomeNameSpace);
                textoArquivo = textoArquivo.Replace("#SIGLA_DAL#", SIGLA_DAL);
                textoArquivo = textoArquivo.Replace("#NOME_CLASSE#", string.Format("{0}{1}", this.nome, SIGLA_DAL));
                textoArquivo = textoArquivo.Replace("#NOME_TABELA#", this.nome);
                textoArquivo = textoArquivo.Replace("#NOME_TABELA_TO#", string.Format("{0}{1}", this.nome, SIGLA_TO));

                sw.Write(textoArquivo);

                sw.Close();
            }
            else
            {
                MessageBox.Show(string.Format("Nenhuma coluna foi encontrada para a tabela {0}", this.nome));
            }
        }

        public void CriarArquivoController()
        {
            if (null != this.colunas && this.colunas.Count > 0)
            {
                String nomeArquivoController = string.Format("{0}Controller.cs", this.nome);
                String pathArquivoController = string.Format("{0}\\{1}", this.pathController, nomeArquivoController);

                if (File.Exists(pathArquivoController))
                {
                    File.Delete(pathArquivoController);
                }

                StreamWriter sw = File.CreateText(pathArquivoController);

                StringBuilder textoArquivo = new StringBuilder(@"using System;
using System.Collections.Generic;
using System.Web.Mvc;
using #NOME_NAMESPACE#.Models.DAL;
using #NOME_NAMESPACE#.Models.TO;

namespace #NOME_NAMESPACE#.Controllers
{
    public class #NOME_CLASSE# : Controller
    {");
                textoArquivo.Append(this.MontarMetodoIndex());
                textoArquivo.Append(this.MontarMetodoGet());
                if (this.tabelaReferenciada)
                {
                    textoArquivo.Append(this.MontarMetodoGetParaChaveEstrangeira());
                }
                textoArquivo.Append(this.MontarMetodoInsert());
                textoArquivo.Append(this.MontarMetodoUpdate());
                textoArquivo.Append(this.MontarMetodoDelete()).Append(@"
    }
}");
                textoArquivo = textoArquivo.Replace("#NOME_NAMESPACE#", nomeNameSpace);
                textoArquivo = textoArquivo.Replace("#NOME_CLASSE#", string.Format("{0}Controller", this.nome));
                textoArquivo = textoArquivo.Replace("#NOME_CLASSE_DAL#", string.Format("{0}{1}", this.nome, SIGLA_DAL));
                textoArquivo = textoArquivo.Replace("#NOME_TABELA#", this.nome);
                textoArquivo = textoArquivo.Replace("#NOME_TABELA_TO#", string.Format("{0}{1}", this.nome, SIGLA_TO));

                sw.Write(textoArquivo);

                sw.Close();
            }
            else
            {
                MessageBox.Show(string.Format("Nenhuma coluna foi encontrada para a tabela {0}", this.nome));
            }
        }

        public void CriarArquivoView()
        {
            if (null != this.colunas && this.colunas.Count > 0)
            {
                String nomeArquivoView = "Index.cshtml";
                String pathArquivoView = string.Format("{0}\\{1}", this.pathView, nomeArquivoView);

                if (File.Exists(pathArquivoView))
                {
                    File.Delete(pathArquivoView);
                }

                StreamWriter sw = File.CreateText(pathArquivoView);

                StringBuilder textoArquivo = new StringBuilder(@"@{
    ViewBag.Title = ""#NOME_TABELA#"";
}
<div class=""row"">
    <div class=""col-lg-12"">
        <div id=""dvTabelaPrincipal"" class=""box"">
            <div class=""box-body"">
                <div style=""margin: 0 0 1em 0;overflow: hidden; text-align:left;"">
                    <button id=""btnNovo"" class=""btn btn-sm btn-success"" type=""button"">
                        <i class=""glyphicon glyphicon-plus visible-xs""></i>
                        <label class=""hidden-xs"" style=""margin:0px;"">Novo item</label>
                    </button>
                </div>
                <div class=""dataTable_wrapper"">
                    <table class=""table table-striped table-bordered table-hover"" id=""tabelaPrincipal"">
                        <thead>
                            <tr>").Append(this.MontarColunasGridView()).Append(@"
                                <th style=""text-align: center;"">A&ccedil;&otilde;es</th>
                            </tr>
                        </thead>
                    </table>
                </div>
            </div>
        </div>
        <div id=""dvFormCadastro"" class=""box"" style=""display:none;"">
            <div class=""box-body"">
                <form id=""formCadastro"" action=""#"">").Append(this.MontarCamposFormulario()).Append(@"
                    <div class=""row"">
                        <div class=""col-lg-12 form-group"" style=""text-align:right;"">
                            <button id=""btnSalvar"" class=""btn btn-sm btn-success"" style=""margin-right:5px;"" type=""button"">
                                <i class=""glyphicon glyphicon-floppy-disk visible-xs""></i>
                                <label class=""hidden-xs"" style=""margin:0px;"">Salvar</label>
                            </button>
                            <button id=""btnSalvarContinuar"" class=""btn btn-sm btn-success"" style=""margin-right:5px;"" type=""button"">
                                <i class=""glyphicon glyphicon-floppy-disk visible-xs""></i>
                                <label class=""hidden-xs"" style=""margin:0px;"">Salvar e Continuar</label>
                            </button>
                            <button id=""btnCancelar"" class=""btn btn-sm btn-danger"" type=""button"">
                                <i class=""glyphicon glyphicon-remove visible-xs""></i>
                                <label class=""hidden-xs"" style=""margin:0px;"">Cancelar</label>
                            </button>
                        </div>
                    </div>
                </form>
            </div>
        </div>
    </div>
</div>

@section script{
    <script src=""@Url.Content(""~/Scripts/custom/#NOME_TABELA_JS#.js"")""></script>
}");
                textoArquivo = textoArquivo.Replace("#NOME_TABELA#", this.nome);
                textoArquivo = textoArquivo.Replace("#NOME_TABELA_JS#", this.nome.ToLower());

                sw.Write(textoArquivo);

                sw.Close();
            }
            else
            {
                MessageBox.Show(string.Format("Nenhuma coluna foi encontrada para a tabela {0}", this.nome));
            }
        }

        public void CriarArquivoJavaScript()
        {
            if (null != this.colunas && this.colunas.Count > 0)
            {
                String nomeArquivoJavaScript = string.Format("{0}.js", this.nome.ToLower());
                String pathArquivoJavaScript = string.Format("{0}\\{1}", this.pathScript, nomeArquivoJavaScript);

                if (File.Exists(pathArquivoJavaScript))
                {
                    File.Delete(pathArquivoJavaScript);
                }

                StreamWriter sw = File.CreateText(pathArquivoJavaScript);

                bool adicionarListaSelects = false;
                StringBuilder textoArquivo = new StringBuilder(this.MontarVariaveisControleChavesEstrangeiras()).Append("var camposFormCadastro = {").Append(this.ObterCamposFormCadastroJavaScript()).Append(@"
};
var colunasTabelaPrincipal = [").Append(this.ObterColunasTabelaPrincipalJavaScript()).Append(@",
    {
        render: renderColunaOpcoes
    }
];
metodoGet = '#NOME_TABELA#/Get';
metodoInsert = '#NOME_TABELA#/Insert';

function ajustarBotoes(somenteLeitura) {
    if (somenteLeitura) {
        $('.campoCadastro').css('display', 'none');
        $('.campoLeitura').css('display', '');

        $('#btnSalvar').css('display', 'none');
        $('#btnCancelar').removeClass('btn-danger');
        $('#btnCancelar').addClass('btn-primary');
        $('#btnCancelar > label').html('Voltar');
    } else {
        $('.campoCadastro').css('display', '');
        $('.campoLeitura').css('display', 'none');

        $('#btnSalvar').css('display', '');
        $('#btnCancelar').removeClass('btn-primary');
        $('#btnCancelar').addClass('btn-danger');
        $('#btnCancelar > label').html('Cancelar');
    }
}
").Append(this.MontarMetodosAjaxListaSelects()).Append(@"
function limparFormCadastro() {
    $('#btnSalvarContinuar').css('display', '');
    $('#btnSalvar').removeData('idRegistro');

    ajustarBotoes(false);
").Append(this.ObterCamposPreenchimentoJavaScript(false, ref adicionarListaSelects)).Append(@"

    mostrarDvFormCadastro($('#dvFormCadastro'), $('#dvTabelaPrincipal')").Append(adicionarListaSelects ? ", listaSelects);" : ");").Append(@"
}

function carregarFormCadastro(").Append(this.ObterParametrosUpdate(false, false)).Append(@") {
    $('#btnSalvarContinuar').css('display', 'none');
    $('#btnSalvar').data('idRegistro', {").Append(this.ObterIdRegistroJavaScript(false, false)).Append(@"
    });

    ajustarBotoes(false);
").Append(this.ObterCamposPreenchimentoJavaScript(true, ref adicionarListaSelects)).Append(@"

    mostrarDvFormCadastro($('#dvFormCadastro'), $('#dvTabelaPrincipal')").Append(adicionarListaSelects ? ", listaSelects);" : ");").Append(@"
}

metodoDetalhes = true;
function exibirDetalhes(").Append(this.ObterParametrosUpdate(false, false)).Append(@") {
    $('#btnSalvarContinuar').css('display', 'none');
    $('#btnSalvar').removeData('idRegistro');

    ajustarBotoes(true);").Append(this.ObterCamposPreenchimentoJavaScriptSomenteLeitura()).Append(@"

    mostrarDvFormCadastro($('#dvFormCadastro'), $('#dvTabelaPrincipal'));").Append(@"
}

metodoDelete = '#NOME_TABELA#/Delete';
function colunasTabelaRemocao(objetoTabela) {
    return '{ ").Append(this.ObterIdRegistroJavaScript(true, false)).Append(@" }';
}

metodoUpdate = '#NOME_TABELA#/Update';
function colunasTabelaAlteracao(objetoTabela) {").Append(this.MontarAuxValorParametrosAlteracao()).Append(@"
    return '").Append(this.ObterParametrosUpdate(false, true)).Append(@"';
}

function montarDadosCadastro() {
    return {").Append(this.MontarObjetoInsert(true)).Append(@"
    };
}

$(document).ready(function () {
    inicializarTabelaPrincipal($('#tabelaPrincipal'), colunasTabelaPrincipal);

    options.fields = camposFormCadastro;
    $('#formCadastro').bootstrapValidator(options);

    $('#btnNovo').click(function () {
        limparFormCadastro();
    });

    $('#btnSalvar').click(function () {
        var auxUrl;
        var auxData = montarDadosCadastro();

        var auxId = $(this).data('idRegistro');
        if (undefined == auxId) {
            auxUrl = metodoInsert;
        } else {
            auxUrl = metodoUpdate;").Append(this.ObterIdRegistroJavaScript(false, true)).Append(@"
        }

        salvarRegistro($('#formCadastro'), $('#dvFormCadastro'), $('#dvTabelaPrincipal'), auxUrl, auxData);
    });

    $('#btnSalvarContinuar').click(function () {
        var auxUrl = metodoInsert;
        var auxData = montarDadosCadastro();
        salvarRegistro($('#formCadastro'), $('#dvFormCadastro'), $('#dvTabelaPrincipal'), auxUrl, auxData, undefined, true);
    });

    $('#btnCancelar').click(function () {
        mostrarDvTabelaPrincipal($('#formCadastro'), $('#dvFormCadastro'), $('#dvTabelaPrincipal'), undefined);
    });
});");

                textoArquivo = textoArquivo.Replace("#NOME_TABELA#", this.nome);

                sw.Write(textoArquivo);

                sw.Close();
            }
            else
            {
                MessageBox.Show(string.Format("Nenhuma coluna foi encontrada para a tabela {0}", this.nome));
            }
        }
    }
}