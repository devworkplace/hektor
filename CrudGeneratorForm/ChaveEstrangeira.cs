using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrudGeneratorForm
{
    public class ChaveEstrangeira
    {
        public String nomeTabelaReferenciada { get; }
        public IDictionary<String, String> colunasChave { get; }

        private ChaveEstrangeira()
        {

        }

        public ChaveEstrangeira(String nomeTabelaReferenciada, IDictionary<String, String> colunasChave)
        {
            this.nomeTabelaReferenciada = nomeTabelaReferenciada;
            this.colunasChave = colunasChave;
        }

        public String MontarIdRegistroEscolhido()
        {
            StringBuilder result = new StringBuilder();
            if (this.colunasChave != null && this.colunasChave.Count > 0)
            {
                IList<String> auxColunasChave = this.colunasChave.Keys.ToList();
                int indice = 0;
                String colunaOrigem = auxColunasChave[indice];
                result.Append(colunaOrigem);
                for (indice = 1; indice < auxColunasChave.Count; indice++)
                {
                    colunaOrigem = auxColunasChave[indice];
                    result.Append(string.Format(" + ';' + {0}", colunaOrigem));
                }
            }
            return result.ToString();
        }

        public String MontarNomeRegistroEscolhido()
        {
            StringBuilder result = new StringBuilder();
            if (this.colunasChave != null && this.colunasChave.Count > 0)
            {
                IList<String> auxColunasChave = this.colunasChave.Keys.ToList();
                int indice = 0;
                String colunaOrigem = auxColunasChave[indice];
                result.Append(colunaOrigem);
                for (indice = 1; indice < auxColunasChave.Count; indice++)
                {
                    colunaOrigem = auxColunasChave[indice];
                    result.Append(string.Format(" + (isNullOrEmpty({0}) ? '' : ' - ' + {0})", colunaOrigem));
                }
            }
            return result.ToString();
        }

        public String MontarParaDadosCadastro(bool jaPossuiAlgumCampoChave)
        {
            StringBuilder result = new StringBuilder(jaPossuiAlgumCampoChave ? "," : string.Empty);
            if (this.colunasChave != null && this.colunasChave.Count > 0)
            {
                IList<String> auxColunasChave = this.colunasChave.Keys.ToList();
                String colunaOrigem;
                if (auxColunasChave.Count > 1)
                {
                    int i;
                    for (i = 0; i < auxColunasChave.Count - 1; i++)
                    {
                        colunaOrigem = auxColunasChave[i];
                        result.Append(@"
        ").Append(string.Format("{0}: $('#{1}').val().toString().split(';')[{2}],", colunaOrigem, this.nomeTabelaReferenciada, i));
                    }
                    i = auxColunasChave.Count - 1;
                    colunaOrigem = auxColunasChave[i];
                    result.Append(@"
        ").Append(string.Format("{0}: $('#{1}').val().toString().split(';')[{2}]", colunaOrigem, this.nomeTabelaReferenciada, i));
                }
                else
                {
                    colunaOrigem = auxColunasChave[0];
                    result.Append(@"
        ").Append(string.Format("{0}: $('#{1}').val()", colunaOrigem, this.nomeTabelaReferenciada));
                }
            }
            return result.ToString();
        }
    }
}
