using System;
using System.Security.Cryptography;
using System.Text;

namespace GeradorQRCodeNFCe
{
    class Program
    {
        static void Main(string[] args)
        {
            string _versaoQRCode = "100";
            string _atributoDest = "&cDest=";
            string _idToken = "";
            string _token = "";
            string _hash = "";
            string _dadosQRCodeSemHash = "chNFe={0}&nVersao={1}&tpAmb={2}{3}&dhEmi={4}&vNF={5}&vICMS={6}&digVal={7}&cIdToken={8}{9}";
            string _dadosQRCode = "{0}chNFe={1}&nVersao={2}&tpAmb={3}{4}&dhEmi={5}&vNF={6}&vICMS={7}&digVal={8}&cIdToken={9}&cHashQRCode={10}";

             Console.WriteLine("Id_CSC: ");
            var id_CSC = Console.ReadLine();
            Console.WriteLine("CSC: ");
            var csc = Console.ReadLine();
            Console.WriteLine("Chave de Acesso: ");
            string ChaveAcesso = Console.ReadLine();
            Console.WriteLine("Data e hora da emissao (ex: 2018-07-10T15:37:25-03:00): ");
            string DataHoraEmissao = Console.ReadLine();
            Console.WriteLine("Valor da NFC-e: ");
            string ValorNF = Console.ReadLine();
            Console.WriteLine("Valor ICMS: ");
            string ValorICMS = Console.ReadLine();
            Console.WriteLine("DigestValue: ");
            string DigestValue = Console.ReadLine();
            Console.WriteLine("Documeto destinatario: ");
            string DocDest = Console.ReadLine();
            Console.WriteLine("Ambiente(1 = Producao, 2 = Homologacao): ");
            int ambiente = Convert.ToInt32(Console.ReadLine());

            if (!string.IsNullOrEmpty(DocDest))
            {
                _atributoDest = _atributoDest + DocDest;
            }
            else _atributoDest = "";

            DataHoraEmissao = StrToHexaStr(DataHoraEmissao);
            DigestValue = StrToHexaStr(DigestValue);

            _dadosQRCodeSemHash = string.Format(_dadosQRCodeSemHash,
                                        ChaveAcesso,
                                        _versaoQRCode,
                                        ambiente,
                                        _atributoDest,
                                        DataHoraEmissao,
                                        ValorNF,
                                        ValorICMS,
                                        DigestValue,
                                        _idToken,
                                        _token);

            //Aplicar o SHA-1
            _hash = StrToSHA1(_dadosQRCodeSemHash);

            //Formatar o QRCode
            _dadosQRCode =
            string.Format(_dadosQRCode, getURIValidacao(ambiente),
                                        ChaveAcesso,
                                        _versaoQRCode,
                                        ((int)(ambiente)).ToString(),
                                        _atributoDest,
                                        DataHoraEmissao.ToLower(),
                                        ValorNF,
                                        ValorICMS,
                                        DigestValue.ToLower(),
                                        _idToken,
                                        _hash.ToUpper());


            Console.WriteLine(_dadosQRCode);
            Console.ReadKey();
        }

        static string StrToHexaStr(string Texto)
        {
            byte[] ba = Encoding.UTF8.GetBytes(Texto);
            var hexString = BitConverter.ToString(ba);
            return hexString.Replace("-", "");
        }

        static string StrToSHA1(string Texto)
        {
            byte[] b = Encoding.UTF8.GetBytes(Texto);
            var sha1 = SHA1.Create();
            byte[] hashByte = sha1.ComputeHash(b);

            var sb = new StringBuilder();

            foreach (byte hb in hashByte)
            {
                var hex = hb.ToString("x2");
                sb.Append(hex);
            }
            return sb.ToString();
        }

        static string getURIValidacao(int ambiente)
        {
            string _URI;
            if (ambiente == 1)
            {
                _URI = "http://app.sefaz.es.gov.br/ConsultaNFCe/qrcode.aspx?";
            }
            else { _URI = "http://homologacao.sefaz.es.gov.br/ConsultaNFCe/qrcode.aspx?"; }
            return _URI;
        }
    }
}