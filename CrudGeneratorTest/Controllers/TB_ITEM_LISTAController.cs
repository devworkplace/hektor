using System;
using System.Collections.Generic;
using System.Web.Mvc;
using CrudGeneratorTest.Models.DAL;
using CrudGeneratorTest.Models.TO;

namespace CrudGeneratorTest.Controllers
{
    public class TB_ITEM_LISTAController : Controller
    {
        public ActionResult Index() 
        {
            return View();
        }
            
        [HttpPost]
        public JsonResult Get()
        {
            int draw = Convert.ToInt32(Request.Form["draw"]);
            int start = Convert.ToInt32(Request.Form["start"]);
            int length = Convert.ToInt32(Request.Form["length"]);
            string textoFiltro = Request.Form["search[value]"];
            string sortColumn = Request.Form[string.Format("columns[{0}][name]", Request.Form["order[0][column]"])];
            string sortColumnDir = Request.Form["order[0][dir]"];

            int totRegistros = 0;
            int totRegistrosFiltro = 0;
            IList<TB_ITEM_LISTATO> dados = TB_ITEM_LISTADAL.Get(start, length, ref totRegistros, textoFiltro, ref totRegistrosFiltro, sortColumn, sortColumnDir);
            if (start > 0 && dados.Count == 0)
            {
                start -= length;
                dados = TB_ITEM_LISTADAL.Get(start, length, ref totRegistros, textoFiltro, ref totRegistrosFiltro, sortColumn, sortColumnDir);
                return Json(new { draw = draw, recordsFiltered = totRegistrosFiltro, recordsTotal = totRegistros, data = dados, voltarPagina = 'S' }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { draw = draw, recordsFiltered = totRegistrosFiltro, recordsTotal = totRegistros, data = dados }, JsonRequestBehavior.AllowGet);
        }
            
        [HttpPost]
        public JsonResult GetParaChaveEstrangeira()
        {
            IList<object> dados = TB_ITEM_LISTADAL.GetParaChaveEstrangeira();
            return Json(new { data = dados }, JsonRequestBehavior.AllowGet);
        }
            
        [HttpPost]
        public JsonResult Insert(String NOME, Int32 ID_TIPO_LISTA, Int32? ID_PAI)
        {
            string auxMsgErro = string.Empty;
            string auxMsgSucesso = string.Empty;

            TB_ITEM_LISTATO obj = new TB_ITEM_LISTATO
            {
                NOME = NOME,
                ID_TIPO_LISTA = ID_TIPO_LISTA,
                ID_PAI = ID_PAI
            };

            if (TB_ITEM_LISTADAL.Insert(obj) == null)
            {
                auxMsgErro = "Falha ao tentar inserir o registro, favor tente novamente";
            }
            else
            {
                auxMsgSucesso = "Registro inserido com sucesso";
            }

            return Json(new { msgErro = auxMsgErro, msgSucesso = auxMsgSucesso });
        }
            
        [HttpPost]
        public JsonResult Update(String NOME, Int32 ID_TIPO_LISTA, Int32? ID_PAI, Int32 ID)
        {
            string auxMsgErro = string.Empty;
            string auxMsgSucesso = string.Empty;

            TB_ITEM_LISTATO obj = new TB_ITEM_LISTATO
            {
                NOME = NOME,
                ID_TIPO_LISTA = ID_TIPO_LISTA,
                ID_PAI = ID_PAI,
                ID = ID
            };

            if (TB_ITEM_LISTADAL.Update(obj) == null)
            {
                auxMsgErro = "Falha ao tentar alterar o registro, favor tente novamente";
            }
            else
            {
                auxMsgSucesso = "Registro alterado com sucesso";
            }

            return Json(new { msgErro = auxMsgErro, msgSucesso = auxMsgSucesso });
        }
            
        [HttpPost]
        public JsonResult Delete(Int32 ID)
        {
            string auxMsgErro = string.Empty;
            string auxMsgSucesso = string.Empty;

            TB_ITEM_LISTATO obj = new TB_ITEM_LISTATO
            {
                ID = ID
            };

            if (TB_ITEM_LISTADAL.Delete(obj) == null)
            {
                auxMsgErro = "Falha ao tentar excluir o registro, favor tente novamente";
            }
            else
            {
                auxMsgSucesso = "Registro excluído com sucesso";
            }

            return Json(new { msgErro = auxMsgErro, msgSucesso = auxMsgSucesso });
        }
    }
}