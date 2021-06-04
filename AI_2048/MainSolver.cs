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

//------------------
//main関数
//------------------
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
        }
        if (field.isValidSlide("Left"))
        {
            GameField field_L = new GameField(field);
            Left = AplphaBetaSerch(field_L.slide("Left"), 1, MinusInff, PlusInf);
        }
        if (field.isValidSlide("Up"))
        {
            GameField field_U = new GameField(field);
            Up = AplphaBetaSerch(field_U.slide("Up"), 1, MinusInff, PlusInf);
        }
        if (field.isValidSlide("Down"))
        {
            GameField field_D = new GameField(field);
            Down = AplphaBetaSerch(field_D.slide("Down"), 1, MinusInff, PlusInf);
        }

        //評価値の出力
        //Console.WriteLine("--------評価値------");
        //Console.WriteLine("Right：{0}",Right);
        //Console.WriteLine("Left：{0}",Left);
        //Console.WriteLine("Up：{0}", Up);
        //Console.WriteLine("Down：{0}", Down);
        //Console.WriteLine("---------------------");
        
        //評価値が最大の方向を返す
        if (Right >= Left && Right >= Down && Right >= Up) return "Right";
        if (Left >= Right && Left >= Down && Left >= Up) return "Left";
        if (Up >= Right && Up >= Down && Up >= Left) return "Up";
        if (Down >= Right && Down >= Up && Down >= Up) return "Down";
        return "Right";
    }


    //--------------------------------------
    //深さ優先探索 minimax探索 alpha-beta
    //-------------------------------------
    public double AplphaBetaSerch(GameField field, int deep, double maxEvaluation, double minEvaluation)//0 , 無限
    {
        //ゲームセットの時
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
        //上方向に移動
        if (fieldUp.isValidSlide("Up"))
        {
            fieldUp.slide("Up");
            List<Tuple<int, int>> L = putPiece(fieldUp);
            if (L.Count<=3)
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
                    if (R > 4) minEvaluationUp = Math.Min(minEvaluationUp, AplphaBetaSerch(fieldUp.putNewPieceAt(L[i], 4), deep, maxEvaluation, minEvaluationUp));
                    else minEvaluationUp = Math.Min(minEvaluationUp, AplphaBetaSerch(fieldUp.putNewPieceAt(L[i], 2), deep, maxEvaluation, minEvaluationUp));
                    //ロスカット
                    if (maxEvaluation >= minEvaluationUp) break;
                }
            }
            maxEvaluation = Math.Max(maxEvaluation, minEvaluationUp);
        }
        //ロスカット
        if (maxEvaluation >= minEvaluation)
        {
            return maxEvaluation;
        }
        //下方向に移動
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
                    if (R > 4) minEvaluationDown = Math.Min(minEvaluationDown, AplphaBetaSerch(fieldDown.putNewPieceAt(L[i], 4), deep, maxEvaluation, minEvaluationDown));
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
        //右方向に移動
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
                    if (R > 4) minEvaluationRight = Math.Min(minEvaluationRight, AplphaBetaSerch(fieldRight.putNewPieceAt(L[i], 4), deep, maxEvaluation, minEvaluationRight));
                    else minEvaluationRight = Math.Min(minEvaluationRight, AplphaBetaSerch(fieldRight.putNewPieceAt(L[i], 2), deep, maxEvaluation, minEvaluationRight));
                    if (maxEvaluation >= minEvaluationRight) break;
                }
            }
            maxEvaluation = Math.Max(maxEvaluation, minEvaluationRight);
        }
        if (maxEvaluation >= minEvaluation)
        {
            return maxEvaluation;
        }
        //左方向に移動
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
                    if (R > 4) minEvaluationLeft = Math.Min(minEvaluationLeft, AplphaBetaSerch(fieldLeft.putNewPieceAt(L[i], 4), deep, maxEvaluation, minEvaluationLeft));
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
        if (L.Count <= 6) return L;
        else
        {
            List<Tuple<int, int>> M = L.OrderBy(i => Guid.NewGuid()).ToList();
            return M.GetRange(0, 6);
        }
    }

    //-------------------------
    //評価関数---盤面を評価
    //-------------------------
    public double Evaluation(GameField field)
    {
        double Piece = 0;
        double Score = 0;
        double NextPiece = 0;

        for (int i=0; i<4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                Tuple<int, int> P = new Tuple<int, int>(i, j);
                //それぞれのピースの2乗の和を計算
                Piece += Math.Pow(field[P], 2);

                //隣接するマスの値の差の合計求める
                if (field[P] != 0)
                {
                    for (int k = 1; k < 4; k++)
                    {
                        Tuple<int, int> toP = new Tuple<int, int>(i + k, j);
                        if (!field.isValidPosition(toP)) break;
                        if (field[toP] != 0)
                        {
                            NextPiece += Math.Abs(Math.Log(field[P], 2) - Math.Log(field[toP], 2));
                            break;
                        }
                    }
                    for (int k = 1; k < 4; k++)
                    {
                        Tuple<int, int> toP = new Tuple<int, int>(i, j + k);
                        if (!field.isValidPosition(toP)) break;
                        if (field[toP] != 0)
                        {
                            NextPiece += Math.Abs( Math.Log(field[P], 2) - Math.Log(field[toP], 2));
                            break;
                        }
                    }
                }
            }
        }
        Score = Piece + (-NextPiece / 2) ;
        return Score;
    }
}