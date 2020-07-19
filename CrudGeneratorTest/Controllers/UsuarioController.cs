using System;
using System.Collections.Generic;
using System.Web.Mvc;
using SETI.Models.DAL;
using SETI.Models.TO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;

namespace SETI.Controllers
{
    public class UsuarioController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Get(string requestModel)
        {
            JArray auxModel = JsonConvert.DeserializeObject(requestModel) as JArray;

            JObject auxDraw = auxModel[0] as JObject;
            JObject auxStart = auxModel[3] as JObject;
            JObject auxLength = auxModel[4] as JObject;

            int draw = Convert.ToInt32(auxDraw.GetValue("value"));
            int start = Convert.ToInt32(auxStart.GetValue("value"));
            int length = Convert.ToInt32(auxLength.GetValue("value"));

            JObject auxSearch = auxModel[5] as JObject;
            JObject auxColumns = auxModel[1] as JObject;
            JObject auxOrder = auxModel[2] as JObject;

            string textoFiltro = Convert.ToString(((JObject)auxSearch.GetValue("value")).GetValue("value"));
            int indiceSortColumn = 0;
            try
            {
                indiceSortColumn = Convert.ToInt32(((JObject)((JArray)auxOrder.GetValue("value"))[0]).GetValue("column"));
            }
            catch { }
            string sortColumn = Convert.ToString(((JObject)((JArray)auxColumns.GetValue("value"))[indiceSortColumn]).GetValue("name"));
            string sortColumnDir = "asc";
            try
            {
                sortColumnDir = Convert.ToString(((JObject)((JArray)auxOrder.GetValue("value"))[0]).GetValue("dir"));
            }
            catch { }

            int totRegistros = 0;
            int totRegistrosFiltro = 0;
            IList<UsuarioTO> dados = UsuarioDAL.Get(start, length, ref totRegistros, textoFiltro, ref totRegistrosFiltro, sortColumn, sortColumnDir);
            if (start > 0 && dados.Count == 0)
            {
                start -= length;
                dados = UsuarioDAL.Get(start, length, ref totRegistros, textoFiltro, ref totRegistrosFiltro, sortColumn, sortColumnDir);
                return Json(new { draw = draw, recordsFiltered = totRegistrosFiltro, recordsTotal = totRegistros, data = dados, voltarPagina = 'S' }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { draw = draw, recordsFiltered = totRegistrosFiltro, recordsTotal = totRegistros, data = dados }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetParaChaveEstrangeira()
        {
            IList<object> dados = UsuarioDAL.GetParaChaveEstrangeira();
            return Json(new { data = dados }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Insert(String login, String senha, String cpf, String nome, String email, Char tipo, Int32? idUsuarioCriador, DateTime? dataAdmissao)
        {
            string auxMsgErro = string.Empty;
            string auxMsgSucesso = string.Empty;

            UsuarioTO obj = new UsuarioTO
            {
                login = login,
                senha = senha,
                cpf = cpf,
                nome = nome,
                email = email,
                tipo = tipo,
                usuarioCriador = null == idUsuarioCriador ? null : new UsuarioTO { id = (Int32)idUsuarioCriador },
                dataAdmissao = dataAdmissao
            };

            if (UsuarioDAL.Insert(obj) == null)
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
        public JsonResult Update(String login, String senha, String cpf, String nome, String email, Char tipo, Int32? idUsuarioCriador, DateTime? dataAdmissao, Int32 id)
        {
            string auxMsgErro = string.Empty;
            string auxMsgSucesso = string.Empty;

            UsuarioTO obj = new UsuarioTO
            {
                login = login,
                senha = senha,
                cpf = cpf,
                nome = nome,
                email = email,
                tipo = tipo,
                usuarioCriador = null == idUsuarioCriador ? null : new UsuarioTO { id = (Int32)idUsuarioCriador },
                dataAdmissao = dataAdmissao,
                id = id
            };

            if (UsuarioDAL.Update(obj) == null)
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
        public JsonResult Delete(Int32 id)
        {
            string auxMsgErro = string.Empty;
            string auxMsgSucesso = string.Empty;

            UsuarioTO obj = new UsuarioTO
            {
                id = id
            };

            if (UsuarioDAL.Delete(obj) == null)
            {
                auxMsgErro = "Falha ao tentar excluir o registro, favor tente novamente";
            }
            else
            {
                auxMsgSucesso = "Registro excluído com sucesso";
            }

            return Json(new { msgErro = auxMsgErro, msgSucesso = auxMsgSucesso });
        }

        [HttpPost]
        public JsonResult ExcluirMultiplos(string[] idsUsuarios)
        {
            string auxMsgErro = string.Empty;
            StringBuilder auxMsgSucesso = null;

            if (idsUsuarios != null && idsUsuarios.Length > 0)
            {
                int? resultExclusao;
                string msgResultExclusao;
                foreach (string idUsuario in idsUsuarios)
                {
                    resultExclusao = UsuarioDAL.Delete(new UsuarioTO { id = Convert.ToInt32(idUsuario) });
                    if (resultExclusao == null)
                    {
                        msgResultExclusao = string.Format("Não foi possível exluir o usuário {0}, favor tente novamente", idUsuario);
                        if (null == auxMsgSucesso)
                        {
                            auxMsgSucesso = new StringBuilder("Operação realizada, porém a(s) seguinte(s) falha(s):");
                        }
                        auxMsgSucesso.Append("<br/>").Append(msgResultExclusao);
                    }
                }
            }
            else
            {
                auxMsgErro = "Falha ao tentar atender: nenhum registro foi selecionado";
            }

            if (null == auxMsgSucesso)
            {
                auxMsgSucesso = new StringBuilder("Operação realizada com sucesso");
            }

            return Json(new { msgErro = auxMsgErro, msgSucesso = auxMsgSucesso.ToString() });
        }
    }
}