using System;
using System.Collections.Generic;
using System.Web.Mvc;
using CrudGeneratorTest.Models.DAL;
using CrudGeneratorTest.Models.TO;

namespace CrudGeneratorTest.Controllers
{
    public class ItemListaController : Controller
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
            IList<ItemListaTO> dados = ItemListaDAL.Get(start, length, ref totRegistros, textoFiltro, ref totRegistrosFiltro, sortColumn, sortColumnDir);
            if (start > 0 && dados.Count == 0)
            {
                start -= length;
                dados = ItemListaDAL.Get(start, length, ref totRegistros, textoFiltro, ref totRegistrosFiltro, sortColumn, sortColumnDir);
                return Json(new { draw = draw, recordsFiltered = totRegistrosFiltro, recordsTotal = totRegistros, data = dados, voltarPagina = 'S' }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { draw = draw, recordsFiltered = totRegistrosFiltro, recordsTotal = totRegistros, data = dados }, JsonRequestBehavior.AllowGet);
        }
            
        [HttpPost]
        public JsonResult GetParaChaveEstrangeira()
        {
            IList<object> dados = ItemListaDAL.GetParaChaveEstrangeira();
            return Json(new { data = dados }, JsonRequestBehavior.AllowGet);
        }
            
        [HttpPost]
        public JsonResult Insert(String Nome, Int32 TipoLista, Int32? IdPai)
        {
            string auxMsgErro = string.Empty;
            string auxMsgSucesso = string.Empty;

            ItemListaTO obj = new ItemListaTO
            {
                Nome = Nome,
                TipoLista = TipoLista,
                IdPai = IdPai
            };

            if (ItemListaDAL.Insert(obj) == null)
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
        public JsonResult Update(String Nome, Int32 TipoLista, Int32? IdPai, Int32 Id)
        {
            string auxMsgErro = string.Empty;
            string auxMsgSucesso = string.Empty;

            ItemListaTO obj = new ItemListaTO
            {
                Nome = Nome,
                TipoLista = TipoLista,
                IdPai = IdPai,
                Id = Id
            };

            if (ItemListaDAL.Update(obj) == null)
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
        public JsonResult Delete(Int32 Id)
        {
            string auxMsgErro = string.Empty;
            string auxMsgSucesso = string.Empty;

            ItemListaTO obj = new ItemListaTO
            {
                Id = Id
            };

            if (ItemListaDAL.Delete(obj) == null)
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