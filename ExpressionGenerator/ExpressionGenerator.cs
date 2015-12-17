using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionGenerator
{
 
    internal class Generator
    {
        internal enum Operations
        { addition, subtraction, multiplication, division, eval, nil, neutral0, neutral1}

        internal Dictionary<Operations, char> mOperations = new Dictionary<Operations, char>
        {
            {Operations.addition, '+'}, 
            {Operations.subtraction,  '-'}, 
            {Operations.multiplication, '*'},
            {Operations.division, '/'}
        };
        internal Random mRandom = new Random();



        private List<int> mValueArray;
        private List<Operations> mOperationArray;
        private int mDepth;
        private int mLimit;
        private int mValueI;
        private int polynom_length;


        internal Generator(int depth, int ivalue, int limit, int length)
        {
            mValueArray = Enumerable.Repeat(0, 2 << depth+2).ToList();
            mOperationArray = Enumerable.Repeat(Operations.nil, 2 << depth+2).ToList();
            mDepth = depth;
            mOperationArray[1] = Operations.eval;
            mValueI = ivalue;
            mLimit = limit;
            polynom_length = length;
        }

        private void split(int position, bool stopLeft, bool stopRight)
        {
            Operations op;
            
            int current = mValueArray[position];
            int value;
            if (stopRight || stopLeft)
            {
                int action = mRandom.Next(2); //0 stands for known variables, 1 for unknown
                if (action == 0)
                {
                    value = mValueI;
                    op = (Operations)mRandom.Next(2);
                    switch (op)
                    {
                        case Operations.addition:
                            {
                                if (stopRight)
                                    add(position, current - value, value);
                                else
                                    add(position, value, current - value);
                                break;
                            }
                        case Operations.subtraction:
                            {
                                if (stopRight)
                                    subs(position, current + value, value);
                                else
                                    subs(position, value, value - current);
                                break;
                            }
                    }

                }
                else
                {
                    value = mRandom.Next(2); // 2 ways for making neutrals : x +- 0 , x*/1
                    if (value == 0)
                    {
                        op = (Operations)mRandom.Next(2);
                        if (stopRight)
                        {
                            mOperationArray[position * 2] = Operations.eval;
                            mOperationArray[position * 2 + 1] = Operations.neutral0;
                        }
                        else
                        {
                            mOperationArray[position * 2] = Operations.neutral0;
                            mOperationArray[position * 2 + 1] = Operations.eval;
                        }
                        switch (op)
                        {
                            case Operations.addition:
                                {
                                    if (stopRight)
                                        makeNeutral(position, current, 0, op);
                                    else
                                        makeNeutral(position, 0, current, op);
                                    break;
                                }
                            case Operations.subtraction:
                                {
                                    if (stopRight)
                                        makeNeutral(position, current, 0, op);
                                    else
                                        makeNeutral(position, 0, -current, op);
                                    break;
                                }
                        }
                    }
                    else
                    {//2 ways x/1 or x/*1
                        op = (Operations)mRandom.Next(2, 4);
                        if (stopRight)
                        {
                            mOperationArray[position * 2] = Operations.eval;
                            mOperationArray[position * 2 + 1] = Operations.neutral1;
                        }
                        else
                        {
                            mOperationArray[position * 2] = Operations.neutral1;
                            mOperationArray[position * 2 + 1] = Operations.eval;
                        }
                        switch (op)
                        {
                            case Operations.multiplication:
                                {
                                    if (stopRight)
                                        makeNeutral(position, current, 1, op);
                                    else
                                        makeNeutral(position, 1, current, op);
                                    break;
                                }
                            case Operations.division:
                                {
                                    if (stopRight)
                                        makeNeutral(position, current, 1, op);
                                    else
                                        makeNeutral(position, 1, current, op);
                                    break;
                                }
                        }
                    }


                }//end make neutrals action

            }//end actions
            else
            {
                if (current != 0)
                    op = (Operations)mRandom.Next(4);
                else
                    op = (Operations)mRandom.Next(2);
                switch (op)
                {
                    case Operations.addition:
                        {
                            value = mRandom.Next(1, mLimit) * (mRandom.Next(2) == 0 ? -1 : 1);//get not 0 value
                            add(position, current - value, value);
                            break;
                        }
                    case Operations.subtraction:
                        {
                            value = mRandom.Next(1, mLimit) * (mRandom.Next(2) == 0 ? -1 : 1);//get not 0 value
                            subs(position, current + value, value);
                            break;
                        }
                    case Operations.multiplication:
                        {
                            mul(position);
                            break;
                        }
                    case Operations.division:
                        {
                            if (!div(position))
                                split(position, stopLeft, stopRight);
                            break;
                        }
                }
            }
        }

        private void add(int position,int leftValue, int RightValue)
        {
            mValueArray[position * 2] = leftValue;
            mValueArray[position * 2 + 1] = RightValue;
            mOperationArray[position] = Operations.addition;
            mOperationArray[position * 2] = Operations.eval;
            mOperationArray[position * 2 + 1] = Operations.eval;
        }

        private void subs(int position,int leftValue, int RightValue)
        {
            mValueArray[position * 2] = leftValue;
            mValueArray[position * 2 + 1] = RightValue;
            mOperationArray[position] = Operations.subtraction;
            mOperationArray[position * 2] = Operations.eval;
            mOperationArray[position * 2 + 1] = Operations.eval;
        }

        private void mul(int position)
        {
            int current = mValueArray[position];
            List<int> divs = Utilities.findDividers(current);
            int divider = divs.DefaultIfEmpty(1).ElementAt(mRandom.Next(divs.Count));
            mValueArray[position * 2] = current / divider;
            mValueArray[position * 2 + 1] = divider;
            mOperationArray[position] = Operations.multiplication;
            mOperationArray[position * 2] = Operations.eval;
            if (divider == 1)
                mOperationArray[position * 2 + 1] = Operations.neutral1;
            else
                mOperationArray[position * 2 + 1] = Operations.eval;


            /* Another way making multiplication
             * value = value % (int)Math.Sqrt(current);
             while (value == 0)
                 value = mRandom.Next((int)Math.Sqrt(current));
             int mod = current % value;
             mValueArray[position * 2] = current - mod;
             mValueArray[position * 2 + 1] = mod;
             mValueArray[position * 4] = current/value;
             mValueArray[position * 4 + 1] = value;

             mOperationArray[position] = Operations.addition;
             mOperationArray[position * 2] = Operations.multiplication;

             mOperationArray[position * 2 + 1] = Operations.eval;
             mOperationArray[position * 4] = Operations.eval;
             mOperationArray[position * 4 + 1] = Operations.eval;*/
        }

        private bool div(int position)
        {
            int current = mValueArray[position];
            List<int> remainders = Utilities.findRemainders(current,mLimit);
            if (remainders.Count == 0)//try again to Split            
                return false;
            int remainder = remainders.ElementAt(mRandom.Next(remainders.Count));
            mValueArray[position * 2] = remainder + mRandom.Next(remainder + (current - 1) > mLimit ? mLimit - remainder : Math.Abs(current));
            mValueArray[position * 2 + 1] = remainder / current;
            mOperationArray[position] = Operations.division;
            mOperationArray[position * 2] = Operations.eval;
            mOperationArray[position * 2 + 1] = Operations.eval;
            return true;
            
        }

       private void makeNeutral(int position,int leftValue, int RightValue, Operations op)
        {
            mValueArray[position * 2] = leftValue;
            mValueArray[position * 2 + 1] = RightValue;
            mOperationArray[position] = op;
        }
        private string split0(char c)
        {
            List<int> first = new List<int>();
            int length1 = mRandom.Next(1, polynom_length);
            for (int i = 0; i < length1; i++)
            {
                first.Add(mRandom.Next(-mLimit/2,mLimit / 2));
            }
            List<int> second = new List<int>();
            int length2 = length1 % 2 == 0 ? length1 + 1 : length1; 
            length2 =  mRandom.Next(length2, polynom_length);
            second.Add(Math.Abs(first[0]) + mRandom.Next(mLimit-first[0]));
            for (int i = 1; i <= length1/2; i++)
            {
                second.Add(0);
                int sum = Math.Abs(first[i * 2 - 1]) + Math.Abs(first.ElementAtOrDefault(i * 2));
                second.Add(sum + mRandom.Next(mLimit - sum));
            }
            for (int i = second.Count; i < length2; i++)
            {
                if (i % 2 == 1)
                    second.Add(0);
                else
                    second.Add(mRandom.Next(mLimit));
            }

            StringBuilder s = new StringBuilder();
            s.Append("(");
            s.Append(Utilities.serializePolynom(first, length1, c));
            s.Append(")/(");
            s.Append(Utilities.serializePolynom(second, length2, c));
            s.Append(")");
            return s.ToString();
        }

        private string split1(char c)
        {
            int length1 = mRandom.Next(2, polynom_length-1);
            int length2 = mRandom.Next(2, polynom_length - length1);
            List<int> first = new List<int>();
            List<int> second = new List<int>();
            for (int i = 0; i < length1; i++)
            {
                first.Add(mRandom.Next(-mLimit / (length1 * length2), mLimit / (length1 * length2)));
            }
            for (int i = 0; i < length2; i++)
            {
                second.Add(mRandom.Next(-mLimit / (length1 * length2), mLimit / (length1 * length2)));
            }
            List<int> result = Enumerable.Repeat(0, length1 + length2).ToList();
            for (int i = 0; i < length1; i++)
                for (int j = 0; j < length2; j++)
                    result[i + j] += first[i] * second[j];

            StringBuilder s = new StringBuilder();
            s.Append("(");
            s.Append(Utilities.serializePolynom(first, length1, c));
            s.Append(")*(");
            s.Append(Utilities.serializePolynom(second, length2, c));
            s.Append(")/(");
            s.Append(Utilities.serializePolynom(result, length1 + length2, c));
            s.Append(")");
            return s.ToString();
        }

        internal void generate(int position = 1)
        {
 
            bool goLeft = (mRandom.Next(3) != 0 && position < 2 << (mDepth - 1)) || position <2;//66% chance not to go deeper
            bool goRight = (mRandom.Next(3) != 0 && position < 2 << (mDepth - 1)) || position < 2;
            split(position, !goRight,  !goLeft);
            if (goLeft)
                generate(position * 2);
            if (goRight)
                 generate(position * 2 + 1);
            
        }

        private string traverse(int pos) 
        {
            if ((mOperationArray[pos] == Operations.neutral0))
                return split0('j');
            else if ((mOperationArray[pos] == Operations.neutral1))
                return split1('j');
            else if (mOperationArray[pos] == Operations.eval)
                return mValueArray[pos] == mValueI ? "i" : mValueArray[pos].ToString();//replace by i
            else if ((int)mOperationArray[pos] < 2 )
                return "(" + traverse(pos * 2) + mOperations[mOperationArray[pos]] + traverse(pos * 2 + 1) + ")";
            else 
                return traverse(pos * 2) + mOperations[mOperationArray[pos]] + traverse(pos * 2 + 1);
        }

        public override string ToString()
        {
            return traverse(1).Replace("--", "+").Replace("+-", "-").Replace("-+", "-");
        }



    }
}
