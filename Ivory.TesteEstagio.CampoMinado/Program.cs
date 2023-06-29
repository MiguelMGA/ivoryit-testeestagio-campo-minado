using System;
using System.Collections.Generic;

namespace Ivory.TesteEstagio.CampoMinado
{
    class Decifrar
    {
        readonly CampoMinado Campo;
        private int IndiceInteracao;

        // Posições das minas no eixo (x, y)
        private HashSet<Tuple<int, int>> PosicoesMinas;

        public Decifrar(CampoMinado campo)
        {
            this.Campo = campo;
            this.IndiceInteracao = 0;
            this.PosicoesMinas = new HashSet<Tuple<int, int>>();
        }

        List<Tuple<int, int>> AnalisarMinas(string[] matrix, int i, int j, HashSet<Tuple<int, int>> posMinas = null)
        {
            var centerNr = matrix[i][j] - '0';
            if (centerNr == 0)
            {
                return null;
            }

            var lugarLivre = new List<Tuple<int, int>>();
            Action<int, int, int> popularLugarLivre = (int coluna, int linhaComeco, int linhaFinal) =>
            {
                for (; linhaComeco <= linhaFinal; ++linhaComeco)
                {
                    if (matrix[coluna][linhaComeco] == '-')
                    {
                        if (posMinas.Contains(new Tuple<int, int>(coluna, linhaComeco)))
                        {
                            // Caso haja uma bomba já encontrada nas posições disponíveis, compensar siminuindo o número de bombas ao redor
                            centerNr--;
                        }
                        else
                        {
                            lugarLivre.Add(new Tuple<int, int>(coluna, linhaComeco));
                        }
                    }
                }
            };

            // Posições na linha de cima
            if (i != 0)
            {
                popularLugarLivre(Math.Max(i - 1, 0), Math.Max(j - 1, 0), Math.Min(j + 1, 8));
            }

            // Posições possíveis na mesma linha
            {
                popularLugarLivre(Math.Max(i, 0), Math.Max(j - 1, 0), Math.Min(j + 1, 8));
            }

            // Posições na linha a baixo
            if (i != 8)
            {
                popularLugarLivre(Math.Max(i + 1, 0), Math.Max(j - 1, 0), Math.Min(j + 1, 8));
            }

             // Caso haja a mesma quantidade de posições livres do que o número indicado pela posição analisada, então todas as posições livres possuem bombas.
            if (lugarLivre.Count == centerNr)
            {
                return lugarLivre;
            }

            return null;
        }

        public bool Terminado()
        {
            return this.Campo.JogoStatus != 0; ;
        }

        public int NivelIteracao()
        {
            return this.IndiceInteracao;
        }

        public void Jogada()
        {
            ++this.IndiceInteracao;

            var matrix = this.Campo.Tabuleiro.Split("\r\n");
            for (int x = 0; x < matrix.Length; ++x)
            {
                for (int y = 0; y < matrix[x].Length; ++y)
                {
                    var c = matrix[x][y];

                    // Uma posição aberta: Verificar se existe uma mina na posição encontrada antes de abrir
                    if (c == '-')
                    {
                        if (((this.IndiceInteracao % 2) == 0) && !this.PosicoesMinas.Contains(new Tuple<int, int>(x, y)))
                        {
                            this.Campo.Abrir(x + 1, y + 1);

                            // Pular a linha após abrir a posição
                            break;
                        }
                    }

                    //Um dígito: Verificar se é possível identificar alguma mina através dele
                    else if (char.IsDigit(c))
                    {
                        var p = this.AnalisarMinas(matrix, x, y, this.PosicoesMinas);
                        if (p != null)
                        {
                            foreach (var l in p)
                            {
                                this.PosicoesMinas.Add(l);
                            }
                        }
                    }
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var campoMinado = new CampoMinado();
            Console.WriteLine("Início do jogo\n=========");
            Console.WriteLine(campoMinado.Tabuleiro);

            // Realize sua codificação a partir deste ponto, boa sorte!
            var decifrar = new Decifrar(campoMinado);
            while (!decifrar.Terminado())
            {
                decifrar.Jogada();

                Console.WriteLine($"\nJogada: {decifrar.NivelIteracao()}\n{campoMinado.Tabuleiro}");
            }

            if (campoMinado.JogoStatus == 1)
            {
                Console.WriteLine($"\nStatus do jogo: {campoMinado.JogoStatus} (Vitória)");
            }
            else if (campoMinado.JogoStatus == 1)
            {
                Console.WriteLine($"\nStatus do jogo: {campoMinado.JogoStatus} (Game Over)");
            }
        }
    }
}
