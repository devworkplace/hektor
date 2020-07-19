using System;

namespace CrudGeneratorTest.Models.TO
{
    public class TB_ITEM_LISTATO
    {
		public Int32 ID { get; set; }
		public String NOME { get; set; }
		public Int32 ID_TIPO_LISTA { get; set; }
		public Int32? ID_PAI { get; set; }
    }
}