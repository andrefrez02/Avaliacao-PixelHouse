﻿namespace CaixaComanda.Classes
{
    public class Usuario
    {
        public string Id { get; set; }
        public string Nome { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
    }
        
    public class CampoGenerico
    {
        public string Campo { get; set; }
        public string Valor { get; set; }
    }
}