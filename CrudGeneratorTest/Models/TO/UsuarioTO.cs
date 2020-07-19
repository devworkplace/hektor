using System;

namespace SETI.Models.TO
{
    public class UsuarioTO
    {
		public Int32 id { get; set; }
		public String login { get; set; }
		public String senha { get; set; }
		public String cpf { get; set; }
		public String nome { get; set; }
		public String email { get; set; }
        public Char tipo { get; set; }
        public String tipoGrid { get; set; }
		public DateTime? dataAdmissao { get; set; }
        public String dataAdmissaoGrid { get; set; }
        public UsuarioTO usuarioCriador { get; set; }
    }
}