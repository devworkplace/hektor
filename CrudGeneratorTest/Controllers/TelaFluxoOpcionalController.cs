using System;
using System.Collections.Generic;
using System.Web.Mvc;
using CrudGeneratorTest.Models.DAL;
using CrudGeneratorTest.Models.TO;

namespace CrudGeneratorTest.Controllers
{
    public class TelaFluxoOpcionalController : Controller
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
            IList<TelaFluxoOpcionalTO> dados = TelaFluxoOpcionalDAL.Get(start, length, ref totRegistros, textoFiltro, ref totRegistrosFiltro, sortColumn, sortColumnDir);
            if (start > 0 && dados.Count == 0)
            {
                start -= length;
                dados = TelaFluxoOpcionalDAL.Get(start, length, ref totRegistros, textoFiltro, ref totRegistrosFiltro, sortColumn, sortColumnDir);
                return Json(new { draw = draw, recordsFiltered = totRegistrosFiltro, recordsTotal = totRegistros, data = dados, voltarPagina = 'S' }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { draw = draw, recordsFiltered = totRegistrosFiltro, recordsTotal = totRegistros, data = dados }, JsonRequestBehavior.AllowGet);
        }
            
        [HttpPost]
        public JsonResult Insert(String OrigemFluxo, String DestinoFluxo)
        {
            string auxMsgErro = string.Empty;
            string auxMsgSucesso = string.Empty;

            TelaFluxoOpcionalTO obj = new TelaFluxoOpcionalTO
            {
                OrigemFluxo = OrigemFluxo,
                DestinoFluxo = DestinoFluxo
            };

            if (TelaFluxoOpcionalDAL.Insert(obj) == null)
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
        public JsonResult Update(String OrigemFluxo, String DestinoFluxo, Int32 Id)
        {
            string auxMsgErro = string.Empty;
            string auxMsgSucesso = string.Empty;

            TelaFluxoOpcionalTO obj = new TelaFluxoOpcionalTO
            {
                OrigemFluxo = OrigemFluxo,
                DestinoFluxo = DestinoFluxo,
                Id = Id
            };

            if (TelaFluxoOpcionalDAL.Update(obj) == null)
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

            TelaFluxoOpcionalTO obj = new TelaFluxoOpcionalTO
            {
                Id = Id
            };

            if (TelaFluxoOpcionalDAL.Delete(obj) == null)
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