using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Generate114514.Functions
{
    public static partial class Algorithms
    {
        public static string SimpleArgument114115(int[] source, int target, CancellationToken cancellationToken, ref double Report)
        {
            if (source.Length < 2) return "Error. Elemens count must great equal than 2.";
            int sourceLength = source.Length;
            RPNOperator[] operatorQueue = new RPNOperator[sourceLength * 2 - 2];
            ComputationData computationData = new ComputationData();
            computationData.operatorQueue = operatorQueue;
            computationData.opValues = source;
            computationData.target = target;
            computationData.cancellationToken = cancellationToken;

            computationData.forBranchCull = new HashSet<int>[sourceLength];
            for (int i = 0; i < computationData.forBranchCull.Length; i++)
            {
                computationData.forBranchCull[i] = new HashSet<int>();
            }
            //computationData.forBranchCull = new SortedSet<int>[sourceLength];
            //for(int i=0;i < computationData.forBranchCull.Length;i++)
            //{
            //    computationData.forBranchCull[i] = new SortedSet<int>();
            //}

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Restart();
            if (_ComputeCancelableWithReport(0, 0, 0, computationData, 1, 0, ref Report))
            {
                stopwatch.Stop();
                return string.Format("{0}={1}\nwith {2} milliseconds.", ToInfix(operatorQueue, source), target, stopwatch.ElapsedMilliseconds);
            }
            else if (cancellationToken.IsCancellationRequested)
            {
                stopwatch.Stop();
                return string.Format("compute was canceled.\nwith {0} milliseconds.", stopwatch.ElapsedMilliseconds);
            }
            else
            {
                stopwatch.Stop();
                return string.Format("cannot find answer.\nwith {0} milliseconds.", stopwatch.ElapsedMilliseconds);
            }
        }

        static string ToInfix(RPNOperator[] operatorQueue, int[] source)
        {
            Stack<PrecedenceExpression> computeStack = new Stack<PrecedenceExpression>();
            Stack<int> sourcesStack = new Stack<int>(source);
            computeStack.Push(new PrecedenceExpression(sourcesStack.Pop()));
            for (int i = 0; i < operatorQueue.Length; i++)
            {
                if (operatorQueue[i] == RPNOperator.Push)
                    computeStack.Push(new PrecedenceExpression(sourcesStack.Pop()));
                else
                {
                    string newContent;
                    PrecedenceExpression content1 = computeStack.Pop();
                    PrecedenceExpression content2 = computeStack.Pop();
                    int currentPriority = operatorPriority[operatorQueue[i]];
                    string symbol = operatorSymbol[operatorQueue[i]];
                    if (content1.priority >= currentPriority && content2.priority >= currentPriority)
                        newContent = string.Format("{0}{2}{1}", content1.content, content2.content, symbol);
                    else if (content1.priority < currentPriority && content2.priority >= currentPriority)
                        newContent = string.Format("({0}){2}{1}", content1.content, content2.content, symbol);
                    else if (content1.priority >= currentPriority && content2.priority < currentPriority)
                        newContent = string.Format("{0}{2}({1})", content1.content, content2.content, symbol);
                    else
                        newContent = string.Format("({0}){2}({1})", content1.content, content2.content, symbol);
                    computeStack.Push(new PrecedenceExpression(newContent, currentPriority));
                }
            }

            return computeStack.Peek().content;
        }

        struct PrecedenceExpression
        {
            public string content;
            public int priority;
            public PrecedenceExpression(int value)
            {
                content = value.ToString();
                priority = int.MaxValue;
            }
            public PrecedenceExpression(string content, int priority)
            {
                this.content = content;
                this.priority = priority;
            }
        }

        class ComputationData
        {
            public int target;
            public RPNOperator[] operatorQueue;
            public int[] opValues;
            public CancellationToken cancellationToken;
            public HashSet<int>[] forBranchCull;
            //public SortedSet<int>[] forBranchCull;
        }

        static bool _Compute(int NumOfA, int NumOfB, int depth, ComputationData computationData)
        {
            ref var operatorQueue = ref computationData.operatorQueue;
            ref var target = ref computationData.target;
            ref var opValues = ref computationData.opValues;

            if (depth < operatorQueue.Length)
            {
                if (NumOfA < operatorQueue.Length >> 1)
                {
                    operatorQueue[depth] = RPNOperator.Push;
                    if (_Compute(NumOfA + 1, NumOfB, depth + 1, computationData)) return true;
                }
                if (NumOfA > NumOfB)
                {
                    operatorQueue[depth] = RPNOperator.Add;
                    if (_Compute(NumOfA, NumOfB + 1, depth + 1, computationData)) return true;
                    operatorQueue[depth] = RPNOperator.Sub;
                    if (_Compute(NumOfA, NumOfB + 1, depth + 1, computationData)) return true;
                    operatorQueue[depth] = RPNOperator.Mul;
                    if (_Compute(NumOfA, NumOfB + 1, depth + 1, computationData)) return true;
                }
            }
            else
            {
                Stack<int> computeStack = new Stack<int>(opValues.Length);
                Stack<int> sourcesStack = new Stack<int>(opValues);
                computeStack.Push(sourcesStack.Pop());
                for (int i = 0; i < operatorQueue.Length; i++)
                {
                    if (operatorQueue[i] == RPNOperator.Push)
                        computeStack.Push(sourcesStack.Pop());
                    else if (operatorQueue[i] == RPNOperator.Add)
                        computeStack.Push(computeStack.Pop() + computeStack.Pop());
                    else if (operatorQueue[i] == RPNOperator.Sub)
                        computeStack.Push(computeStack.Pop() - computeStack.Pop());
                    else if (operatorQueue[i] == RPNOperator.Mul)
                        computeStack.Push(computeStack.Pop() * computeStack.Pop());
                    else if (operatorQueue[i] == RPNOperator.Div)
                        throw new NotImplementedException();
                }
                if (computeStack.Peek() == target) return true;
            }
            return false;
        }

        const int c_performaceValue = 5;
        static bool _ComputeCancelableWithReport(int NumOfA, int NumOfB, int depth, ComputationData computationData, double weight, double baseValue, ref double Report)
        {
            ref var operatorQueue = ref computationData.operatorQueue;
            ref var opValues = ref computationData.opValues;
            ref var target = ref computationData.target;
            ref var cancellationToken = ref computationData.cancellationToken;
            ref var forBranchCull = ref computationData.forBranchCull;

            if (depth != 0 && NumOfA == NumOfB)
            {
                Stack<int> computeStack = new Stack<int>(opValues.Length);
                Stack<int> sourcesStack = new Stack<int>(opValues);
                computeStack.Push(sourcesStack.Pop());
                for (int i = 0; i < depth; i++)
                {
                    if (operatorQueue[i] == RPNOperator.Push)
                        computeStack.Push(sourcesStack.Pop());
                    else if (operatorQueue[i] == RPNOperator.Add)
                        computeStack.Push(computeStack.Pop() + computeStack.Pop());
                    else if (operatorQueue[i] == RPNOperator.Sub)
                        computeStack.Push(computeStack.Pop() - computeStack.Pop());
                    else if (operatorQueue[i] == RPNOperator.Mul)
                        computeStack.Push(computeStack.Pop() * computeStack.Pop());
                    else if (operatorQueue[i] == RPNOperator.Div)
                        throw new NotImplementedException();
                }
                int computeValue = computeStack.Peek();
                if (!forBranchCull[depth >> 1].Add(computeValue))
                {
                    Report = baseValue + weight;
                    return false;
                }
                long testValue = Math.Abs(computeValue);
                int sourcesStackCount = sourcesStack.Count;
                long testValue2 = 1;
                for (int i = 0; i < sourcesStackCount; i++)
                {
                    testValue2 = Math.Max(Math.Abs(opValues[i]), 2) * testValue2;
                }
                if (testValue * testValue2 < Math.Abs(target))
                    return false;
                if (testValue - testValue2 > Math.Abs(target))
                    return false;

            }

            int taskComplete = 0;
            double subWeight = weight / (Convert.ToInt32(NumOfA < operatorQueue.Length) * 3 + Convert.ToInt32(NumOfA > NumOfB) * 3);
            if (NumOfA > NumOfB)
            {
                if (operatorQueue.Length - depth < c_performaceValue)
                {
                    operatorQueue[depth] = RPNOperator.Add;
                    if (_Compute(NumOfA, NumOfB + 1, depth + 1, computationData)) return true;
                    operatorQueue[depth] = RPNOperator.Sub;
                    if (_Compute(NumOfA, NumOfB + 1, depth + 1, computationData)) return true;
                    operatorQueue[depth] = RPNOperator.Mul;
                    if (_Compute(NumOfA, NumOfB + 1, depth + 1, computationData)) return true;
                }
                else
                {
                    operatorQueue[depth] = RPNOperator.Add;
                    if (_ComputeCancelableWithReport(NumOfA, NumOfB + 1, depth + 1, computationData, subWeight, baseValue + subWeight * taskComplete, ref Report)) return true;
                    taskComplete++;
                    if (cancellationToken.IsCancellationRequested) return false;
                    operatorQueue[depth] = RPNOperator.Sub;
                    if (_ComputeCancelableWithReport(NumOfA, NumOfB + 1, depth + 1, computationData, subWeight, baseValue + subWeight * taskComplete, ref Report)) return true;
                    taskComplete++;
                    if (cancellationToken.IsCancellationRequested) return false;
                    operatorQueue[depth] = RPNOperator.Mul;
                    if (_ComputeCancelableWithReport(NumOfA, NumOfB + 1, depth + 1, computationData, subWeight, baseValue + subWeight * taskComplete, ref Report)) return true;
                    taskComplete++;
                }
            }
            if (NumOfA < operatorQueue.Length >> 1)
            {
                operatorQueue[depth] = RPNOperator.Push;
                if (operatorQueue.Length - depth < c_performaceValue)
                {
                    if (_Compute(NumOfA + 1, NumOfB, depth + 1, computationData)) return true;
                    if (cancellationToken.IsCancellationRequested) return false;
                }
                else
                {
                    if (_ComputeCancelableWithReport(NumOfA + 1, NumOfB, depth + 1, computationData, subWeight * 3, baseValue + subWeight * taskComplete, ref Report)) return true;
                    taskComplete += 3;
                    if (cancellationToken.IsCancellationRequested) return false;
                }
            }
            Report = baseValue + weight;
            return false;
        }
    }
}
