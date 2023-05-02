using System.Reflection;
using System.Text;

namespace MinhasNoticias
{
    internal class Arquivo
    {
        private readonly string _caminho;

        public Arquivo()
        {
            _caminho = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (!File.Exists(_caminho + @"\sites.txt"))
            {
                using (FileStream fs = File.Create(_caminho + @"\sites.txt"))
                {
                    byte[] info = new UTF8Encoding(true).GetBytes("");
                    fs.Write(info, 0, info.Length);
                }
            }
        }

        public static bool Criar(string nome)
        {
            var a = new Arquivo();
            var nomearquivo = a._caminho + @"\" + nome;

            try
            {
                if (!File.Exists(nomearquivo)){
                    using (FileStream fs = File.Create(nomearquivo)) {
                        byte[] info = new UTF8Encoding(true).GetBytes("");
                        fs.Write(info, 0, info.Length);
                    }
                }
                return true;
            }
            catch (Exception)
            {
            }
            return false;
        }

        public static bool Escrever(string arquivo, string txt)
        {
            var a = new Arquivo();

            try
            {
                using (FileStream SourceStream = File.Open(a._caminho + @"\" + arquivo, FileMode.OpenOrCreate))
                {
                    SourceStream.Seek(0, SeekOrigin.End);
                    byte[] result = new UTF8Encoding(true).GetBytes("\n"+txt);
                    SourceStream.Write(result, 0, Math.Min(txt.Length + 999, result.Length));
                }

                return true;
            }
            catch (Exception)
            {
            }

            return false;
        }

        public static bool Contem(string arquivo, string txt)
        {
            var a = new Arquivo();
            var nomedoarquivo = a._caminho + @"\" + arquivo;
            bool ret = true;

            if (!File.Exists(nomedoarquivo))
            {
                ret = Criar(nomedoarquivo);
            }

            if (ret)
            {
                try
                {
                    using (StreamReader sr = new StreamReader(nomedoarquivo))
                    {
                        string coteudo = sr.ReadToEnd();
                        if (coteudo.Contains(txt)) return true;
                    }
                }
                catch (Exception)
                {
                }
            }

            return false;
        }

        public static string Ler(string arquivo)
        {
            var a = new Arquivo();
            var nomedoarquivo = a._caminho + @"\" + arquivo;

            using (StreamReader sr = new StreamReader(nomedoarquivo))
            {
                return sr.ReadToEnd().ToString();
            }
        }
    }
}
