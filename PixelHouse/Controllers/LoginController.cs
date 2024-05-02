using Microsoft.AspNetCore.Mvc;
using CaixaComanda.Classes;
using CaixaComanda.Models;

namespace CaixaComanda.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            if (Request.Cookies["LoginId"] != null)
            {
                TempData["LoginId"] = Request.Cookies["LoginId"];
            }
            else
            {
                TempData["LoginId"] = "error";
            }

            return View("Lista");
        }

        public IActionResult Login()
        {
            DataModel dados = new DataModel();

            Usuario login = new Usuario();
            string email = string.Empty;
            string senha = string.Empty;
            string erro = string.Empty;

            try
            {
                email = Request.Form["txtEmail"];
                senha = Request.Form["txtSenha"];
            }
            catch (Exception ex)
            {
                erro = ex.Message.ToString();
            }

            login = dados.LoginUsuario(email, senha);

            if (login != null)
            {
                CookieOptions cookieOptions = new CookieOptions()
                {
                    Path = "/",
                    HttpOnly = false,
                    IsEssential = true,
                    Expires = DateTime.Now.AddDays(1)
                };

                Response.Cookies.Append("LoginId", login.Id, cookieOptions);

                TempData["LoginId"] = login.Id;

                return View("../Login/Lista");
            }
            else
            {
                return View();
            }
        }

        public IActionResult Cadastrar()
        {
            DataModel dados = new DataModel();

            string id = string.Empty;

            CampoGenerico nome = new CampoGenerico() { Campo = "Nome" };
            CampoGenerico dataNascimento = new CampoGenerico() { Campo = "DataNascimento" };
            CampoGenerico email = new CampoGenerico() { Campo = "Email" };
            CampoGenerico senha = new CampoGenerico() { Campo = "Senha" };
            CampoGenerico confirmarSenha = new CampoGenerico() { Campo = "ConfirmarSenha" };

            string erro = string.Empty;

            try
            {
                nome.Valor = Request.Form["txtNome"].ToString();
                dataNascimento.Valor = string.Format("{0}{1}{2}", "STR_TO_DATE('", DateTime.Parse(Request.Form["txtDataNascimento"].ToString()).Date.ToString(@"dd-MM-yyyy"), "', '%d-%m-%Y')");
                email.Valor = Request.Form["txtEmail"].ToString();
                senha.Valor = Request.Form["txtSenha"].ToString();
                confirmarSenha.Valor = Request.Form["txtConfirmarSenha"].ToString();
            }
            catch (Exception ex)
            {
                erro = ex.Message.ToString();
            }

            if (senha.Valor != confirmarSenha.Valor)
            {
                erro = "As senhas estão diferentes.";
            }

            if (string.IsNullOrEmpty(erro))
            {
                List<CampoGenerico> parametros = new List<CampoGenerico>();

                parametros.Add(nome);
                parametros.Add(dataNascimento);
                parametros.Add(email);
                parametros.Add(senha);

                id = dados.CadastrarUsuario(parametros);
            }

            if (!string.IsNullOrEmpty(id))
            {
                return View("../Login/Login");
            }
            else
            {
                return View();
            }
        }

        public IActionResult Alterar()
        {
            DataModel dados = new DataModel();

            Usuario usuario = new Usuario();

            string id = string.Empty;

            if (Request.Cookies["LoginId"] != null)
            {
                TempData["LoginId"] = Request.Cookies["LoginId"];
                id = TempData["LoginId"].ToString();
            }
            else
            {
                TempData["LoginId"] = "error";
            }

            usuario = dados.PesquisarUsuario(id);

            CampoGenerico nome = new CampoGenerico() { Campo = "Nome", Valor = usuario.Nome };
            CampoGenerico dataNascimento = new CampoGenerico() { Campo = "DataNascimento", Valor = usuario.DataNascimento.ToString() };
            CampoGenerico senha = new CampoGenerico() { Campo = "Senha", Valor = usuario.Senha };
            CampoGenerico novaSenha = new CampoGenerico() { Campo = "Senha", Valor = usuario.Senha };

            string erro = string.Empty;

            try
            {
                nome.Valor = Request.Form["txtNome"].ToString();
                dataNascimento.Valor = string.Format("{0}{1}{2}", "STR_TO_DATE('", DateTime.Parse(Request.Form["txtDataNascimento"].ToString()).Date.ToString(@"dd-MM-yyyy"), "', '%d-%m-%Y')");
                senha.Valor = Request.Form["txtSenha"].ToString();
                novaSenha.Valor = Request.Form["txtNovaSenha"].ToString();
            }
            catch (Exception ex)
            {
                erro = ex.Message.ToString();
            }

            if (senha.Valor != usuario.Senha)
            {
                erro = "A senha atual está incorreta.";
            }

            if (novaSenha.Valor == senha.Valor)
            {
                erro = "As senhas devem ser diferentes.";
            }

            if (string.IsNullOrEmpty(erro))
            {
                if (novaSenha.Valor != usuario.Senha)
                {
                    List<CampoGenerico> parametros = new List<CampoGenerico>();

                    parametros.Add(nome);
                    parametros.Add(dataNascimento);
                    parametros.Add(novaSenha);

                    dados.AlterarUsuario(parametros, id);
                }
                else
                {
                    erro = "Nenhum campo foi alterado";
                }
            }

            if (!string.IsNullOrEmpty(id) || !string.IsNullOrEmpty(erro))
            {
                return View();
            }
            else
            {
                return View("../Login/Lista");
            }
        }

        public IActionResult Deletar()
        {
            DataModel dados = new DataModel();

            string id = string.Empty;

            if (Request.Cookies["LoginId"] != null)
            {
                TempData["LoginId"] = Request.Cookies["LoginId"];
                id = Request.Cookies["LoginId"];

                CookieOptions cookieOptions = new CookieOptions()
                {
                    Path = "/",
                    HttpOnly = false,
                    IsEssential = true,
                    Expires = DateTime.Now.AddDays(-1)
                };

                dados.DeletarUsuario(id);

                Response.Cookies.Append("LoginId", Request.Cookies["LoginId"], cookieOptions);

                return View("login");
            }
            else
            {
                TempData["LoginId"] = "error";
            }

            return View("login");   
        }

        public IActionResult Deslogar()
        {
            if (Request.Cookies["LoginId"] != null)
            {
                CookieOptions cookieOptions = new CookieOptions()
                {
                    Path = "/",
                    HttpOnly = false,
                    IsEssential = true,
                    Expires = DateTime.Now.AddDays(-1)
                };

                Response.Cookies.Append("LoginId", Request.Cookies["LoginId"], cookieOptions);

                return View("Lista");
            }

            return View("Lista");
        }
    }
}