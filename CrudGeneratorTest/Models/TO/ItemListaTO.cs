using System;

namespace CrudGeneratorTest.Models.TO
{
    public class ItemListaTO
    {
		public Int32 Id { get; set; }
		public String Nome { get; set; }
		public Int32 TipoLista { get; set; }
		public Int32? IdPai { get; set; }
    }
}