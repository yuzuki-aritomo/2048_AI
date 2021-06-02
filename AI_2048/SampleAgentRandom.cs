using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

public class RandomPlacer : PlaceAgent
{
    public RandomPlacer(Random random) : base(random) { }
    public override Tuple<Tuple<int, int>, int> place(List<GameField> history)
    {
        GameField latest = history[history.Count - 1];
        Tuple<int, int>[] candidates =
        Enumerable.Range(0, latest.size.Item1).SelectMany(row => Enumerable.Range(0, latest.size.Item2), (first, second) => new Tuple<int, int>(first, second)).Where(x => latest.isValidPiecePlacement(x)).ToArray();
        return new Tuple<Tuple<int, int>, int>(candidates[rand.Next(candidates.Length)], rand.Next(5) == 0 ? 4 : 2);
    }
}

//main関数
public class MainSolver : SolveAgent
{
    public MainSolver(Random random) : base(random) { }
    //-------------------------
    //main 次の方向を返す
    //-------------------------
    public override string solve(List<GameField> history)
    {
        GameField latest = history[history.Count - 1];
        GameField field =  new GameField(latest);
        Console.WriteLine(Evaluation(field));

        String s = FindSlide(field);
        //4方向に移動可能かどうか判断
        if (latest.isValidSlide(s))
        {
            return s;
        }
        else
        {
            //File.AppendAllText(@"C:\Users\ngnls\Desktop\output.txt", latest.maxpiece.ToString() + "\n");
            //return "end";
            if (latest.isValidSlide("Right"))
            {
                return "Right";
            }else if (latest.isValidSlide("Up"))
            {
                return "Up";
            }
            else if (latest.isValidSlide("Down"))
            {
                return "Down";
            }
            return "Left";
        }
    }

    //-------------------------
    //次の方向を決める
    //-------------------------
    public String FindSlide(GameField field)
    {
        double Right = double.NegativeInfinity;
        double Left = double.NegativeInfinity;
        double Up = double.NegativeInfinity;
        double Down = double.NegativeInfinity;

        double PlusInf = double.PositiveInfinity;
        double MinusInff = double.NegativeInfinity;

        if (field.isValidSlide("Right"))
        {
            GameField field_R = new GameField(field);
            Right = AplphaBetaSerch(field_R.slide("Right"), 1, MinusInff, PlusInf);
            //Right = DepthFirstSearch(field_R.slide("Right"), 1);
        }
        if (field.isValidSlide("Left"))
        {
            GameField field_L = new GameField(field);
            Left = AplphaBetaSerch(field_L.slide("Left"), 1, MinusInff, PlusInf);
            //Left = DepthFirstSearch(field_L.slide("Left"), 1);
        }
        if (field.isValidSlide("Up"))
        {
            GameField field_U = new GameField(field);
            Up = AplphaBetaSerch(field_U.slide("Up"), 1, MinusInff, PlusInf);
            //Up = DepthFirstSearch(field_U.slide("Up"), 1);
        }
        if (field.isValidSlide("Down"))
        {
            GameField field_D = new GameField(field);
            Down = AplphaBetaSerch(field_D.slide("Down"), 1, MinusInff, PlusInf);
            //Down = DepthFirstSearch(field_D.slide("Down"), 1);
        }
        Console.WriteLine("--------評価値------");
        Console.WriteLine("Right：{0}",Right);
        Console.WriteLine("Left：{0}",Left);
        Console.WriteLine("Up：{0}", Up);
        Console.WriteLine("Down：{0}", Down);
        Console.WriteLine("---------------------");
        //評価値が最大のものを返す
        if (Right >= Left && Right >= Down && Right >= Up) return "Right";
        if (Left >= Right && Left >= Down && Left >= Up) return "Left";
        if (Up >= Right && Up >= Down && Up >= Left) return "Up";
        if (Down >= Right && Down >= Up && Down >= Up) return "Down";
        return "Right";
    }


    //--------------------------------------
    //深さ優先探索 max-min探索 alpha-beta探索
    //-------------------------------------
    public double AplphaBetaSerch(GameField field, int deep, double maxEvaluation, double minEvaluation)//0 , 無限
    {
        if (field.checkmate())
        {
            return double.NegativeInfinity;
        }
        if (deep == 5)
        {
            return Evaluation(field);
        }
        
        //深度の追加
        deep++;
        //4方向に動かした場合
        GameField fieldUp = new GameField(field);
        GameField fieldDown = new GameField(field);
        GameField fieldRight = new GameField(field);
        GameField fieldLeft = new GameField(field);

        double minEvaluationUp = minEvaluation;
        double minEvaluationDown = minEvaluation;
        double minEvaluationRight = minEvaluation;
        double minEvaluationLeft = minEvaluation;

        //4個ランダムで譜面置く、一度だけ4, それ以外は2

        if (fieldUp.isValidSlide("Up"))
        {
            fieldUp.slide("Up");
            List<Tuple<int, int>> L = putPiece(fieldUp);
            if (L.Count <= 3)
            {
                for (int i = 0; i < L.Count; i++)
                {
                    GameField fieldUpTwo = new GameField(fieldUp);
                    GameField fieldUpFour = new GameField(fieldUp);
                    minEvaluationUp = Math.Min(minEvaluationUp, AplphaBetaSerch(fieldUpTwo.putNewPieceAt(L[i], 2), deep, maxEvaluation, minEvaluationLeft));
                    if (maxEvaluation >= minEvaluationUp) break;
                    minEvaluationUp = Math.Min(minEvaluationUp, AplphaBetaSerch(fieldUpFour.putNewPieceAt(L[i], 4), deep, maxEvaluation, minEvaluationLeft));
                    if (maxEvaluation >= minEvaluationUp) break;
                }
            }
            else
            {
                for (int i = 0; i < L.Count; i++)
                {
                    int R = rand.Next(1, 10);
                    if (R > 7) minEvaluationUp = Math.Min(minEvaluationUp, AplphaBetaSerch(fieldUp.putNewPieceAt(L[i], 4), deep, maxEvaluation, minEvaluationUp));
                    else minEvaluationUp = Math.Min(minEvaluationUp, AplphaBetaSerch(fieldUp.putNewPieceAt(L[i], 2), deep, maxEvaluation, minEvaluationUp));
                    //ロスカット
                    if (maxEvaluation >= minEvaluationUp) break;
                }
            }
            maxEvaluation = Math.Max(maxEvaluation, minEvaluationUp);
        }
        if (maxEvaluation >= minEvaluation)
        {
            return maxEvaluation;
        }

        if (fieldDown.isValidSlide("Down"))
        {
            fieldDown.slide("Down");
            List<Tuple<int, int>> L = putPiece(fieldDown);
            if (L.Count <= 3)
            {
                for (int i = 0; i < L.Count; i++)
                {
                    GameField fieldDownTwo = new GameField(fieldDown);
                    GameField fieldDownFour = new GameField(fieldDown);
                    minEvaluationDown = Math.Min(minEvaluationDown, AplphaBetaSerch(fieldDownTwo.putNewPieceAt(L[i], 2), deep, maxEvaluation, minEvaluationLeft));
                    if (maxEvaluation >= minEvaluationDown) break;
                    minEvaluationDown = Math.Min(minEvaluationDown, AplphaBetaSerch(fieldDownFour.putNewPieceAt(L[i], 4), deep, maxEvaluation, minEvaluationLeft));
                    if (maxEvaluation >= minEvaluationDown) break;
                }
            }
            else
            {
                for (int i = 0; i < L.Count; i++)
                {
                    int R = rand.Next(1, 10);
                    if (R > 7) minEvaluationDown = Math.Min(minEvaluationDown, AplphaBetaSerch(fieldDown.putNewPieceAt(L[i], 4), deep, maxEvaluation, minEvaluationDown));
                    else minEvaluationDown = Math.Min(minEvaluationDown, AplphaBetaSerch(fieldDown.putNewPieceAt(L[i], 2), deep, maxEvaluation, minEvaluationDown));
                    //ロスカット
                    if (maxEvaluation >= minEvaluationDown) break;
                }
            }
            maxEvaluation = Math.Max(maxEvaluation, minEvaluationDown);
        }
        if (maxEvaluation >= minEvaluation)
        {
            return maxEvaluation;
        }

        if (fieldRight.isValidSlide("Right"))
        {
            fieldRight.slide("Right");
            List<Tuple<int, int>> L = putPiece(fieldRight);
            if (L.Count <= 3)
            {
                for (int i = 0; i < L.Count; i++)
                {
                    GameField fieldRightTwo = new GameField(fieldRight);
                    GameField fieldRightFour = new GameField(fieldRight);
                    minEvaluationRight = Math.Min(minEvaluationRight, AplphaBetaSerch(fieldRightTwo.putNewPieceAt(L[i], 2), deep, maxEvaluation, minEvaluationLeft));
                    if (maxEvaluation >= minEvaluationRight) break;
                    minEvaluationRight = Math.Min(minEvaluationRight, AplphaBetaSerch(fieldRightFour.putNewPieceAt(L[i], 4), deep, maxEvaluation, minEvaluationLeft));
                    if (maxEvaluation >= minEvaluationRight) break;
                }
            }
            else
            {
                for (int i = 0; i < L.Count; i++)
                {
                    int R = rand.Next(1, 10);
                    if (R > 7) minEvaluationRight = Math.Min(minEvaluationRight, AplphaBetaSerch(fieldRight.putNewPieceAt(L[i], 4), deep, maxEvaluation, minEvaluationRight));
                    else minEvaluationRight = Math.Min(minEvaluationRight, AplphaBetaSerch(fieldRight.putNewPieceAt(L[i], 2), deep, maxEvaluation, minEvaluationRight));
                    //ロスカット
                    if (maxEvaluation >= minEvaluationRight) break;
                }
            }
            maxEvaluation = Math.Max(maxEvaluation, minEvaluationRight);
        }
        if (maxEvaluation >= minEvaluation)
        {
            return maxEvaluation;
        }

        if (fieldLeft.isValidSlide("Left"))
        {
            fieldLeft.slide("Left");
            List<Tuple<int, int>> L = putPiece(fieldLeft);
            if (L.Count <= 3)
            {
                for (int i = 0; i < L.Count; i++)
                {
                    GameField fieldLeftTwo = new GameField(fieldLeft);
                    GameField fieldLeftFour = new GameField(fieldLeft);
                    minEvaluationLeft = Math.Min(minEvaluationLeft, AplphaBetaSerch(fieldLeftTwo.putNewPieceAt(L[i], 2), deep, maxEvaluation, minEvaluationLeft));
                    if (maxEvaluation >= minEvaluationDown) break;
                    minEvaluationLeft = Math.Min(minEvaluationLeft, AplphaBetaSerch(fieldLeftFour.putNewPieceAt(L[i], 4), deep, maxEvaluation, minEvaluationLeft));
                    if (maxEvaluation >= minEvaluationLeft) break;
                }
            }
            else
            {
                for (int i = 0; i < L.Count; i++)
                {
                    int R = rand.Next(1, 10);
                    if (R > 7) minEvaluationLeft = Math.Min(minEvaluationLeft, AplphaBetaSerch(fieldLeft.putNewPieceAt(L[i], 4), deep, maxEvaluation, minEvaluationLeft));
                    else minEvaluationLeft = Math.Min(minEvaluationLeft, AplphaBetaSerch(fieldLeft.putNewPieceAt(L[i], 2), deep, maxEvaluation, minEvaluationLeft));
                    if (maxEvaluation >= minEvaluationLeft) break;
                }
            }
            maxEvaluation = Math.Max(maxEvaluation, minEvaluationLeft);
        }
        if (maxEvaluation >= minEvaluation)
        {
            return maxEvaluation;
        }
        //4方向の評価値の最大値を返す
        return maxEvaluation;
    }
    //-------------------------
    //深さ優先探索 全探索 平均と最大値
    //-------------------------
    public double DepthFirstSearch(GameField field, int deep)
    {
        if(deep == 2)
        {
            return Evaluation(field);
        }
        deep++;
        //4方向に動かした場合
        GameField fieldUp = new GameField(field);
        GameField fieldDown = new GameField(field);
        GameField fieldRight = new GameField(field);
        GameField fieldLeft = new GameField(field);

        //4個ランダムで譜面置く、一度だけ4, それ以外は2
        int Up = 0;
        double Evaluation_Up = double.PositiveInfinity;
        if (fieldUp.isValidSlide("Up"))
        { 
            fieldUp.slide("Up");
            List<Tuple<int, int>> L = putPiece(fieldUp);
            for (int i = 0; i < L.Count; i++)
            {
                int R = rand.Next(1, 10);
                if (R > 7) Evaluation_Up = Math.Min(Evaluation_Up, DepthFirstSearch(fieldUp.putNewPieceAt(L[i], 4), deep));
                else Evaluation_Up = Math.Min(Evaluation_Up, DepthFirstSearch(fieldUp.putNewPieceAt(L[i], 2), deep));
            }
        }
        else
        {
            Evaluation_Up = 0;
        }
        int Down = 0;
        double Evaluation_Down = double.PositiveInfinity;
        if (fieldDown.isValidSlide("Down"))
        {
            fieldDown.slide("Down");
            List<Tuple<int, int>> L = putPiece(fieldDown);
            for (int i = 0; i < L.Count; i++)
            {
                int R = rand.Next(1, 10);
                if (R > 7) Evaluation_Down = Math.Min(Evaluation_Down, DepthFirstSearch(fieldDown.putNewPieceAt(L[i], 4), deep));
                else Evaluation_Down = Math.Min(Evaluation_Down, DepthFirstSearch(fieldDown.putNewPieceAt(L[i], 2), deep));
            }
        }
        else
        {
            Evaluation_Down = 0;
        }

        int Right = 0;
        double Evaluation_Right = double.PositiveInfinity;
        if (fieldRight.isValidSlide("Right"))
        {
            fieldRight.slide("Right");
            List<Tuple<int, int>> L = putPiece(fieldRight);
            for (int i = 0; i < L.Count; i++)
            {
                int R = rand.Next(1, 10);
                if (R > 7) Evaluation_Right = Math.Min(Evaluation_Right, DepthFirstSearch(fieldRight.putNewPieceAt(L[i], 4), deep));
                else Evaluation_Right = Math.Min(Evaluation_Right, DepthFirstSearch(fieldRight.putNewPieceAt(L[i], 2), deep));
            }
            //if (L.Count != 0) Evaluation_Right = Right / L.Count;
        }
        else
        {
            Evaluation_Right = 0;
        }

        int Left = 0;
        double Evaluation_Left = double.PositiveInfinity;
        if (fieldLeft.isValidSlide("Left"))
        {
            fieldLeft.slide("Left");
            List<Tuple<int, int>> L = putPiece(fieldLeft);
            for (int i = 0; i < L.Count; i++)
            {
                int R = rand.Next(1, 10);
                if (R > 7) Evaluation_Left = Math.Min(Evaluation_Left, DepthFirstSearch(fieldLeft.putNewPieceAt(L[i], 4), deep));
                else Evaluation_Left = Math.Min(Evaluation_Left, DepthFirstSearch(fieldLeft.putNewPieceAt(L[i], 2), deep));
            }
            //if (L.Count != 0) Evaluation_Left = Left / L.Count;
        }
        else
        {
            Evaluation_Left = 0;
        }
        //if (n == 0) return 0;
        //4方向の評価値の平均の最大値を返す
        return Math.Max(Math.Max(Evaluation_Up, Evaluation_Down), Math.Max(Evaluation_Right, Evaluation_Left));
    }
    //-------------------------
    //ピースを置けるマスをランダムに4つ返す
    //-------------------------
    public List<Tuple<int, int>> putPiece(GameField field)
    {
        //0 の座標をリストLに追加
        var L = new List<Tuple<int, int>>();
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                Tuple<int, int> P = new Tuple<int, int>(i, j);
                if(field[P] == 0)
                {
                    L.Add(P);
                }
            }
        }
        //Lをランダムして最初の4つを返す
        if (L.Count <= 5) return L;
        else
        {
            List<Tuple<int, int>> M = L.OrderBy(i => Guid.NewGuid()).ToList();
            return M.GetRange(0, 5);
        }
        //return L;
    }
    //-------------------------
    //評価関数---盤面を評価
    //-------------------------
    public double Evaluation(GameField field)
    {
        int[,] Piece_Weight =
        {
            {8192, 16384, 32768, 65536},
            {4096, 2048, 1024, 512},
            {32, 64, 128, 256},
            {16, 8, 4, 2},
        };
        double FieldWight = 0;
        double Piece = 0;
        double Score = 0;
        int NextPiece = 0;
        int NonePiece = 0;
        int CanMove = 0;
        double CanMergePiece = 0;
        double NextSpacePiece = 0;
        double MaxValue = 0;

        MaxValue = field.maxpiece;

        //動ける回数
        if (field.isValidSlide("Up")) CanMove++;
        if (field.isValidSlide("Down")) CanMove++;
        if (field.isValidSlide("Right")) CanMove++;
        if (field.isValidSlide("Left")) CanMove++;

        double PieceTF = 0;

        for (int i=0; i<4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                Tuple<int, int> P = new Tuple<int, int>(i, j);
                //Piece = Math.Log(field[P]) * 4;
                Piece += Math.Pow(field[P], 2);
                //FieldWight += field[P] * Piece_Weight[i,j];
                //Piece += field[P];

                if (field[P] == 2 || field[P] == 4)
                {
                    PieceTF--;
                }

                if (field[P] != 0)
                {
                    Tuple<int, int> toP_1 = new Tuple<int, int>(i + 1, j);
                    Tuple<int, int> toP_2 = new Tuple<int, int>(i, j + 1);
                    Tuple<int, int> toP_3 = new Tuple<int, int>(i, j - 1);
                    Tuple<int, int> toP_4 = new Tuple<int, int>(i - 1, j);
                    if(field.isValidPosition(toP_1) && field[toP_1]==0) NextSpacePiece += (1 / field[P]) * 4096;
                    if(field.isValidPosition(toP_2) && field[toP_2]==0) NextSpacePiece += (1 / field[P]) * 4096;
                    if(field.isValidPosition(toP_3) && field[toP_3]==0) NextSpacePiece += (1 / field[P]) * 4096;
                    if(field.isValidPosition(toP_4) && field[toP_4]==0) NextSpacePiece += (1 / field[P]) * 4096;
                }
                else
                {
                    NonePiece++;
                }

                //隣接するマスの値の差の合計求める
                if (field[P] != 0)
                {
                    for (int k = 1; k < 4; k++)
                    {
                        Tuple<int, int> toP = new Tuple<int, int>(i + k, j);
                        if (!field.isValidPosition(toP)) break;
                        if (field[toP] != 0)
                        {
                            NextPiece += Math.Abs(field[P] - field[toP])/4;
                            break;
                        }
                    }
                    for (int k = 1; k < 4; k++)
                    {
                        Tuple<int, int> toP = new Tuple<int, int>(i, j + k);
                        if (!field.isValidPosition(toP)) break;
                        if (field[toP] != 0)
                        {
                            NextPiece += Math.Abs(field[P] - field[toP])/4;
                            break;
                        }
                    }
                }
                if (field[P] != 0)
                {
                    for (int k = 1; k < 4; k++)
                    {
                        Tuple<int, int> toP = new Tuple<int, int>(i + k, j);
                        if (!field.isValidPosition(toP)) break;
                        if (field[toP] == field[P])
                        {
                            CanMergePiece += Math.Pow(field[P], 2);
                            break;
                        }
                    }
                    for (int k = 1; k < 4; k++)
                    {
                        Tuple<int, int> toP = new Tuple<int, int>(i, j + k);
                        if (!field.isValidPosition(toP)) break;
                        if (field[toP] == field[P])
                        {
                            CanMergePiece += Math.Pow(field[P], 2);
                            break;
                        }
                    }
                }
                
                
                

            }
        }
        double monotonicity = 0;
        double[] mono = new double[4];
        for (int i = 0; i < 4; i++)
        {
            for (int k = 0; k < 3; k++)
            {
                Tuple<int, int> P3 = new Tuple<int, int>(i, k);
                Tuple<int, int> P4 = new Tuple<int, int>(i, k+1);
                double valueP3 = field[P3];
                double valueP4 = field[P4];
                if (valueP3 == 0) valueP3 = 1;
                if (valueP4 == 0) valueP4 = 1;
                if (valueP3 == valueP4)
                {
                    continue;
                }
                else if(valueP3 > valueP4)
                {
                    mono[2] += Math.Log(valueP4, 2) - Math.Log(valueP3, 2);
                }
                else
                {
                    mono[3] += Math.Log(valueP3, 2) - Math.Log(valueP4, 2);
                }
            }
        }
        for (int i = 0; i < 4; i++)
        {
            for (int k = 0; k < 3; k++)
            {
                Tuple<int, int> P3 = new Tuple<int, int>(k, i);
                Tuple<int, int> P4 = new Tuple<int, int>(k + 1, i);
                double valueP3 = field[P3];
                double valueP4 = field[P4];
                if (valueP3 == 0) valueP3 = 1;
                if (valueP4 == 0) valueP4 = 1;
                if (valueP3 == valueP4)
                {
                    continue;
                }
                else if (valueP3 > valueP4)
                {
                    mono[0] += Math.Log(valueP4, 2) - Math.Log(valueP3, 2);
                }
                else
                {
                    mono[1] += Math.Log(valueP3, 2) - Math.Log(valueP4, 2);
                }
            }
            monotonicity = Math.Max(Math.Max(mono[0], mono[1]), Math.Max(mono[2], mono[3]));
        }

        //Score = NonePiece * 128 + Piece + CanMove*256 + CanMergePiece;
        //Score = Piece - NextPiece/2;
        //Score = Piece - NextPiece;
        double tmp = 0;
        if (NonePiece == 0)
        {
            tmp = 0;
        }else
        {
            tmp = Math.Log(NonePiece) * 2.7;
        }
        //Console.WriteLine(mono[0]);
        //Score = Math.Log(MaxValue, 2) - NextPiece*0.1 + tmp + monotonicity;
        Score = Piece - NextPiece + NextSpacePiece;
        //Score = NonePiece +  MaxValue + FieldWight -  NextPiece + monotonicity;
        return Score;
    }
}