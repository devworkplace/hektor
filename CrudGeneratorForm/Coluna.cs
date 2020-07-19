using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrudGeneratorForm
{
    public class Coluna
    {
        public string nome { get; }
        public string tipo { get; }
        public string tipoEmCsharp { get; }
        public bool campoObrigatorio { get; }
        public bool autoIncremento { get; }
        public string indice { get; }
        public int tamanho { get; }
        public bool tipoTexto { get; }

        private string ObterTipoColuna(bool campoObrigatorio)
        {
            switch (this.tipo.ToLower())
            {
                case "date":
                    return string.Format("DateTime{0}", campoObrigatorio ? "" : "?");
                case "datetime":
                    return string.Format("DateTime{0}", campoObrigatorio ? "" : "?");
                case "datetime2":
                    return string.Format("DateTime{0}", campoObrigatorio ? "" : "?");
                case "float":
                    return string.Format("float{0}", campoObrigatorio ? "" : "?");
                case "int":
                    return string.Format("Int32{0}", campoObrigatorio ? "" : "?");
                case "bigint":
                    return string.Format("Int64{0}", campoObrigatorio ? "" : "?");
                case "real":
                    return string.Format("Double{0}", campoObrigatorio ? "" : "?");
                case "text":
                    return "String";
                case "varchar":
                    return "String";
                default:
                    return string.Empty;
            }
        }

        public Coluna(string nome, string tipo, bool campoObrigatorio, bool autoIncremento, string indice, int tamanho)
        {
            this.nome = nome;
            this.tipo = tipo;
            this.tipoEmCsharp = this.ObterTipoColuna(campoObrigatorio);
            this.campoObrigatorio = campoObrigatorio;
            this.autoIncremento = autoIncremento;
            this.indice = indice;
            this.tamanho = tamanho;
            this.tipoTexto = "String".Equals(this.tipoEmCsharp);
        }

        public String MontarCampoColuna(int posicao)
        {
            switch (this.tipo.ToLower())
            {
                case "date":
                    return this.campoObrigatorio ?
                        string.Format("{0} = rd.GetDateTime({1})", this.nome, posicao)
                        : string.Format("{0} = rd.IsDBNull({1}) ? (DateTime?)null : rd.GetDateTime({1})", this.nome, posicao);
                case "datetime":
                    return this.campoObrigatorio ?
                        string.Format("{0} = rd.GetDateTime({1})", this.nome, posicao)
                        : string.Format("{0} = rd.IsDBNull({1}) ? (DateTime?)null : rd.GetDateTime({1})", this.nome, posicao);
                case "datetime2":
                    return this.campoObrigatorio ?
                        string.Format("{0} = rd.GetDateTime({1})", this.nome, posicao)
                        : string.Format("{0} = rd.IsDBNull({1}) ? (DateTime?)null : rd.GetDateTime({1})", this.nome, posicao);
                case "float":
                    return this.campoObrigatorio ? 
                        string.Format("{0} = rd.GetFloat({1})", this.nome, posicao)
                        : string.Format("{0} = rd.IsDBNull({1}) ? (float?)null : rd.GetFloat({1})", this.nome, posicao);
                case "int":
                    return this.campoObrigatorio ? 
                        string.Format("{0} = rd.GetInt32({1})", this.nome, posicao)
                        : string.Format("{0} = rd.IsDBNull({1}) ? (Int32?)null : rd.GetInt32({1})", this.nome, posicao);
                case "bigint":
                    return this.campoObrigatorio ?
                        string.Format("{0} = rd.GetInt64({1})", this.nome, posicao)
                        : string.Format("{0} = rd.IsDBNull({1}) ? (Int64?)null : rd.GetInt64({1})", this.nome, posicao);
                case "real":
                    return this.campoObrigatorio ?
                        string.Format("{0} = rd.GetDouble({1})", this.nome, posicao)
                        : string.Format("{0} = rd.IsDBNull({1}) ? (Double?)null : rd.GetDouble({1})", this.nome, posicao);
                case "text":
                    return this.campoObrigatorio ?
                        string.Format("{0} = rd.GetString({1})", this.nome, posicao)
                        : string.Format("{0} = rd.IsDBNull({1}) ? null : rd.GetString({1})", this.nome, posicao);
                case "varchar":
                    return this.campoObrigatorio ?
                        string.Format("{0} = rd.GetString({1})", this.nome, posicao)
                        : string.Format("{0} = rd.IsDBNull({1}) ? null : rd.GetString({1})", this.nome, posicao);
                default:
                    return string.Empty;
            }
        }

        public String ColunaParametroController()
        {
            switch (this.tipo.ToLower())
            {
                case "date":
                    return string.Format("DateTime{0} {1}", this.campoObrigatorio ? "" : "?", this.nome);
                case "datetime":
                    return string.Format("DateTime{0} {1}", this.campoObrigatorio ? "" : "?", this.nome);
                case "datetime2":
                    return string.Format("DateTime{0} {1}", this.campoObrigatorio ? "" : "?", this.nome);
                case "float":
                    return string.Format("float{0} {1}", this.campoObrigatorio ? "" : "?", this.nome);
                case "int":
                    return string.Format("Int32{0} {1}", this.campoObrigatorio ? "" : "?", this.nome);
                case "bigint":
                    return string.Format("Int64{0} {1}", this.campoObrigatorio ? "" : "?", this.nome);
                case "real":
                    return string.Format("Double{0} {1}", this.campoObrigatorio ? "" : "?", this.nome);
                case "text":
                    return string.Format("String {0}", this.nome);
                case "varchar":
                    return string.Format("String {0}", this.nome);
                default:
                    return string.Empty;
            }
        }
    }
}
