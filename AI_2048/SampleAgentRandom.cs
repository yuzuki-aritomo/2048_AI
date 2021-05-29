using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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

//main�֐�
public class MainSolver : SolveAgent
{
    public MainSolver(Random random) : base(random) { }
    //-------------------------
    //main ���̕�����Ԃ�
    //-------------------------
    public override string solve(List<GameField> history)
    {
        GameField latest = history[history.Count - 1];
        GameField field =  new GameField(latest);

        String s = FindSlide(field);
        //4�����Ɉړ��\���ǂ������f
        if (latest.isValidSlide(s))
        {
            return s;
        }
        else
        {
            Console.WriteLine("dame");
            if (latest.isValidSlide("Right")) return "Right";
            else if (latest.isValidSlide("Left")) return "Left";
            else if (latest.isValidSlide("Down")) return "Down";
            else return "Up";
        }
    }

    //-------------------------
    //���̕��������߂�
    //-------------------------
    public String FindSlide(GameField field)
    {
        double Right = 0;
        double Left = 0;
        double Up = 0;
        double Down = 0;
        if (field.isValidSlide("Right"))
        {
            GameField field_R = new GameField(field);
            Right = DepthFirstSearch(field_R.slide("Right"), 1);
        }
        if (field.isValidSlide("Left"))
        {
            GameField field_L = new GameField(field);
            Left = DepthFirstSearch(field_L.slide("Left"), 1);
        }
        if (field.isValidSlide("Up"))
        {
            GameField field_U = new GameField(field);
            Up = DepthFirstSearch(field_U.slide("Up"), 1);
        }
        if (field.isValidSlide("Down"))
        {
            GameField field_D = new GameField(field);
            Down = DepthFirstSearch(field_D.slide("Down"), 1);
        }
        Console.WriteLine("-----�]���l------");
        Console.WriteLine("Right�F{0}",Right);
        Console.WriteLine("Left�F{0}",Left);
        Console.WriteLine("Up�F{0}", Up);
        Console.WriteLine("Down�F{0}", Down);
        Console.WriteLine("-----------");
        //�]���l���ő�̂��̂�Ԃ�
        if (Right >= Left && Right >= Down && Right >= Up) return "Right";
        if (Left >= Right && Left >= Down && Left >= Up) return "Left";
        if (Up >= Right && Up >= Down && Up >= Left) return "Up";
        if (Down >= Right && Down >= Up && Down >= Up) return "Down";
        return "Right";
    }
    //-------------------------
    //�[���D��T��
    //-------------------------
    public double DepthFirstSearch(GameField field, int deep)
    {
        //���ꂼ��̕����̏d��
        double w_Right = 1;
        double w_Left = 1;
        double w_Up = 1;
        double w_Down = 1;
        List<double> scoreList = new List<double>();
        if(deep == 5)
        {
            return Evaluation(field);
        }
        deep++;
        //4�����ɓ��������ꍇ
        GameField fieldUp = new GameField(field);
        GameField fieldDown = new GameField(field);
        GameField fieldRight = new GameField(field);
        GameField fieldLeft = new GameField(field);

        //4�����_���ŕ��ʒu���A��x����4, ����ȊO��2
        double Up = 0;
        double Evaluation_Up = 0;
        if (fieldUp.isValidSlide("Up"))
        { 
            fieldUp.slide("Up");
            List<Tuple<int, int>> L = putPiece(fieldUp);
            for (int i = 0; i < L.Count; i++)
            {
                int R = rand.Next(1, 10);
                if (R > 7) Up += DepthFirstSearch(fieldUp.putNewPieceAt(L[i], 4), deep) * w_Up;
                else Up +=  DepthFirstSearch(fieldUp.putNewPieceAt(L[i], 2), deep) * w_Up;
            }
            if( L.Count != 0 ) Evaluation_Up = Up / L.Count;
        }
        double Down = 0;
        double Evaluation_Down = 0;
        if (fieldDown.isValidSlide("Down"))
        {
            fieldDown.slide("Down");
            List<Tuple<int, int>> L = putPiece(fieldDown);
            for (int i = 0; i < L.Count; i++)
            {
                int R = rand.Next(1, 10);
                if (R > 7) Down += DepthFirstSearch(fieldDown.putNewPieceAt(L[i], 4), deep) * w_Down;
                else Down += DepthFirstSearch(fieldDown.putNewPieceAt(L[i], 2), deep) * w_Down;
            }
            if (L.Count != 0) Evaluation_Down = Down / L.Count;
        }
        double Right = 0;
        double Evaluation_Right = 0;
        if (fieldRight.isValidSlide("Right"))
        {
            fieldRight.slide("Right");
            List<Tuple<int, int>> L = putPiece(fieldRight);
            for (int i = 0; i < L.Count; i++)
            {
                int R = rand.Next(1, 10);
                if (R > 7) Right += DepthFirstSearch(fieldRight.putNewPieceAt(L[i], 4), deep) * w_Right;
                else Right += DepthFirstSearch(fieldRight.putNewPieceAt(L[i], 2), deep) * w_Right;
            }
            if (L.Count != 0) Evaluation_Right = Right / L.Count;
        }
        double Left = 0;
        double Evaluation_Left = 0;
        if (fieldLeft.isValidSlide("Left"))
        {
            fieldLeft.slide("Left");
            List<Tuple<int, int>> L = putPiece(fieldLeft);
            for (int i = 0; i < L.Count; i++)
            {
                int R = rand.Next(1, 10);
                if (R > 7) Left += DepthFirstSearch(fieldLeft.putNewPieceAt(L[i], 4), deep) * w_Left;
                else Left += DepthFirstSearch(fieldLeft.putNewPieceAt(L[i], 2), deep) * w_Left;
            }
            if (L.Count != 0) Evaluation_Left = Left / L.Count;
        }
        //if (n == 0) return 0;
        //4�����̕]���l�̕��ς̍ő�l��Ԃ�
        return Math.Max(Math.Max(Evaluation_Up, Evaluation_Down), Math.Max(Evaluation_Right, Evaluation_Left));
    }
    //-------------------------
    //�s�[�X��u����}�X�������_����4�Ԃ�
    //-------------------------
    public List<Tuple<int, int>> putPiece(GameField field)
    {
        //0 �̍��W�����X�gL�ɒǉ�
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
        //L�������_�����čŏ���4��Ԃ�
        if (L.Count <= 4) return L;
        else
        {
            List<Tuple<int, int>> M = L.OrderBy(i => Guid.NewGuid()).ToList();
            return M.GetRange(0, 5);
        }
    }
    //-------------------------
    //�]���֐�---�Ֆʂ�]��
    //-------------------------
    public double Evaluation(GameField field)
    {
        double[,] squere_weight = {
            {1, 2, 3, 4},
            {2, 3, 4, 5},
            {3, 4, 5, 6},
            {4, 5, 6, 7}
        };
        double E = 0;
        double NextPiece = 0;
        for (int i=0; i<4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                Tuple<int, int> P = new Tuple<int, int>(i, j);
                //E += field[P] * squere_weight[i, j];
                //E += Math.Pow(field[P], 2);
                E += field[P] * 2;
                //�אڂ���}�X�̒l�̍��̍��v���߂�
                if (field[P] != 0)
                {
                    for (int k = 1; k < 4; k++)
                    {
                        Tuple<int, int> toP = new Tuple<int, int>(i + k, j);
                        if (!field.isValidPosition(toP)) break;
                        if (field[toP] != 0)
                        {
                            NextPiece -= Math.Abs(field[P] - field[toP]);
                            break;
                        }
                    }
                    for (int k = 1; k < 4; k++)
                    {
                        Tuple<int, int> toP = new Tuple<int, int>(i, j + k);
                        if (!field.isValidPosition(toP)) break;
                        if (field[toP] != 0)
                        {
                            NextPiece += Math.Abs(field[P] - field[toP]);
                            break;
                        }
                    }
                }
            }
        }
        return E - NextPiece/2;
    }
}