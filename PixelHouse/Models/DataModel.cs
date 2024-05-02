using MySql.Data.MySqlClient;
using System.Data;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using CaixaComanda.Classes;
using System.ComponentModel.DataAnnotations;
using Org.BouncyCastle.Asn1.Ocsp;

namespace CaixaComanda.Models
{
    public class DataModel
    {
        private string GetConnectionString()
        {
            return new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory).AddJsonFile("appsettings.json").Build().GetConnectionString("MyConnectionString");
        }

        private string InjectQuery(string query)
        {
            DataTable dt = new DataTable();
            using (MySqlConnection connection = new MySqlConnection(GetConnectionString()))
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                    {
                        try
                        {
                            adapter.Fill(dt);
                        }
                        catch (Exception ex)
                        {
                            return ex.Message;
                        }
                    }
                }
            }

            return JsonConvert.SerializeObject(dt, Formatting.Indented);
        }

        public List<Usuario> ListarUsuarios()
        {
            return JsonConvert.DeserializeObject<List<Usuario>>(new DataModel().InjectQuery("select * from tb_usuarios;"));
        }

        public Usuario LoginUsuario(string email, string senha)
        {
            Usuario login = new Usuario();

            login = JsonConvert.DeserializeObject<Usuario>(new DataModel().InjectQuery(string.Format("select * from tb_usuarios where Email = '{0}' and Senha = '{1}';", email, senha)).Replace("[", string.Empty).Replace("]", string.Empty));

            return login;
        }

        public Usuario PesquisarUsuario(string id)
        {
            Usuario usuario = new Usuario();

            usuario = JsonConvert.DeserializeObject<Usuario>(new DataModel().InjectQuery(string.Format("select * from tb_usuarios where Id = '{0}';", id)).Replace("[", string.Empty).Replace("]", string.Empty));

            return usuario;
        }

        public void AlterarUsuario(List<CampoGenerico> parametros, string id)
        {
            string valores = string.Empty;
            int i = 0;

            foreach (CampoGenerico item in parametros)
            {
                if (item.Campo == "DataNascimento")
                {
                    valores = i == 0 ? string.Format("{0} = {1}", item.Campo, item.Valor) : string.Format("{0}, {1} = {2}", valores, item.Campo, item.Valor);
                }
                else
                {
                    valores = i == 0 ? string.Format("{0} = '{1}'", item.Campo, item.Valor.Replace("'", "`").Replace("\"", "``")) : string.Format("{0}, {1} = '{2}'", valores, item.Campo, item.Valor.Replace("'", "`").Replace("\"", "``"));
                }
                i++;
            }

            new DataModel().InjectQuery(string.Format("update tb_usuarios set {0} where Id = {1};", valores, id));
        }

        public string CadastrarUsuario(List<CampoGenerico> parametros)
        {
            string valores = string.Empty;
            string campos = string.Empty;
            int i = 0;

            foreach (CampoGenerico item in parametros)
            {
                campos = i == 0 ? string.Format("{0}", item.Campo) : string.Format("{0}, {1}", campos, item.Campo);

                if (item.Campo == "DataNascimento")
                {
                    valores = i == 0 ? string.Format("{0}", item.Valor) : string.Format("{0}, {1}", valores, item.Valor);
                }
                else
                {
                    valores = i == 0 ? string.Format("'{0}'", item.Valor) : string.Format("{0}, '{1}'", valores, item.Valor);
                }
                i++;
            }

            string id = string.Empty;

            id = new DataModel().InjectQuery(string.Format("insert into tb_usuarios ({0}) values ({1}); SELECT LAST_INSERT_ID() as IdCadastrado from tb_usuarios LIMIT 1;", campos, valores));

            id = Regex.Replace(id, @"[\r\n\s{}\[\]]", string.Empty);
            id = Regex.Replace(id, @"""", string.Empty).Replace("IdCadastrado:", string.Empty);

            return id;
        }

        public string DeletarUsuario(string id)
        {
            bool existe = Regex.Replace(new DataModel().InjectQuery(string.Format("select Id from tb_usuarios where Id = {0};", id)), @"[\r\n\s{}\[\]]", string.Empty).Length > 0;

            if (existe)
            {
                new DataModel().InjectQuery(string.Format(string.Format("delete from tb_usuarios where Id = {0};", id)));

                existe = Regex.Replace(new DataModel().InjectQuery(string.Format("select Id from tb_usuarios where Id = {0};", id)), @"[\r\n\s{}\[\]]", string.Empty).Length > 0;

                if (!existe)
                {
                    return "Usuário deletado com sucesso.";
                }
                else
                {
                    return "Não foi possível deletar o usuário.";
                }
            }
            else
            {
                return "Usuário não encontrado.";
            }
        }
    }
}