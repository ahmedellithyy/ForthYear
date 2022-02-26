using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tiny_Compiler
{
    public class Node
    {
        public List<Node> Children = new List<Node>();

        public string Name;
        public Node(string N)
        {
            this.Name = N;
        }
    }
    public class Parser
    {
        int InputPointer = 0;
        List<Token> TokenStream;
        public Node root;
        public Node StartParsing(List<Token> TokenStream)
        {
            this.InputPointer = 0;
            this.TokenStream = TokenStream;
            root = new Node("Program");
            root.Children.Add(Program());
            return root;
        }
        Node Program()
        {
            Node program = new Node("Program");
            program.Children.Add(Fun_statD());
            program.Children.Add(Main());
            return program;
        }
        Node Fun_statD()
        {
            Node Fun_statDN = new Node("Functions");
            if (InputPointer >= TokenStream.Count)
                return null;
            if (!((TokenStream[InputPointer].token_type == Token_Class.DATATYPE_FLOAT ||
                TokenStream[InputPointer].token_type == Token_Class.DATATYPE_INT ||
                TokenStream[InputPointer].token_type == Token_Class.DATATYPE_STRING) &&
                TokenStream[InputPointer + 1].token_type == Token_Class.MAIN))
            {
                Node tmpFun_statDN = Fun_stat();
                Fun_statDN.Children.Add(tmpFun_statDN);
                if (!(tmpFun_statDN == null))
                    Fun_statDN.Children.Add(Fun_statD());
            }
            else
                Fun_statDN = null;
            return Fun_statDN;
        }
        Node Main()
        {
            Node main = new Node("Main");
            if (InputPointer >= TokenStream.Count)
            {
                Compiler.Syntax_Errors.Add("Parsing Error: Program Doesn't Contain Exactly One main Function");
                return null;
            }
            if (TokenStream[InputPointer].token_type == Token_Class.DATATYPE_FLOAT)
                main.Children.Add(match(Token_Class.DATATYPE_FLOAT));
            else if(TokenStream[InputPointer].token_type == Token_Class.DATATYPE_INT)
                main.Children.Add(match(Token_Class.DATATYPE_INT));
            else if (TokenStream[InputPointer].token_type == Token_Class.DATATYPE_STRING)
            main.Children.Add(match(Token_Class.DATATYPE_STRING));
            main.Children.Add(match(Token_Class.MAIN));
            main.Children.Add(match(Token_Class.LPARENT));
            main.Children.Add(match(Token_Class.RPARENT));
            main.Children.Add(Fun_Bod());
            return main;
        }
        Node Fun_stat()
        {
            Node Fun_statN = new Node("Function");
            Fun_statN.Children.Add(Fun_Decl());
            Fun_statN.Children.Add(Fun_Bod());
            return Fun_statN;
        }

        Node Fun_Bod()
        {
            Node Fun_BodN = new Node("Function Body");
            Fun_BodN.Children.Add(match(Token_Class.LBRACES));
            Fun_BodN.Children.Add(Stats_SetD());
            Fun_BodN.Children.Add(Return());
            while(true)
            {
                if (InputPointer >= TokenStream.Count - 1 || TokenStream[InputPointer].token_type == Token_Class.RBRACES)
                    break;
                else if (TokenStream[InputPointer].token_type == Token_Class.COMMENT)
                {
                    InputPointer++;
                    continue;
                }
                else
                    match(TokenStream[InputPointer].token_type);
                Compiler.Syntax_Errors.Add("Warning: The Following Token Is Ignored " + TokenStream[InputPointer - 1].token_type.ToString() + " At TokenStream Index #" + (InputPointer - 1) + " Reason: Function Already Returned");
            }
            Fun_BodN.Children.Add(match(Token_Class.RBRACES));
            return Fun_BodN;
        }

        Node Fun_Decl()
        {
            Node Fun_DeclN = new Node("Function Declaration");
            if(TokenStream[InputPointer].token_type == Token_Class.DATATYPE_FLOAT)
                Fun_DeclN.Children.Add(match(Token_Class.DATATYPE_FLOAT));
            if (TokenStream[InputPointer].token_type == Token_Class.DATATYPE_INT)
                Fun_DeclN.Children.Add(match(Token_Class.DATATYPE_INT));
            if (TokenStream[InputPointer].token_type == Token_Class.DATATYPE_STRING)
                Fun_DeclN.Children.Add(match(Token_Class.DATATYPE_STRING));
            Fun_DeclN.Children.Add(match(Token_Class.IDENTIFIER));
            Fun_DeclN.Children.Add(match(Token_Class.LPARENT));
            Fun_DeclN.Children.Add(Para());
            Fun_DeclN.Children.Add(ParaD());
            Fun_DeclN.Children.Add(match(Token_Class.RPARENT));
            return Fun_DeclN;
        }

        Node Stats_SetD()
        {
            Node Stats_SetDN = new Node("Statements");
            Node tmpStats = Stats_Set();
            Stats_SetDN.Children.Add(tmpStats);
            if(!(tmpStats == null))
                Stats_SetDN.Children.Add(Stats_SetD());
            return Stats_SetDN;
        }
        Node Stats_Set()
        {
            Node Stats_SetN = new Node("Statement");
            if (InputPointer >= TokenStream.Count)
                return null;
            if (TokenStream[InputPointer].token_type == Token_Class.IDENTIFIER)
                Stats_SetN.Children.Add(Assign_Stat());
            else if (TokenStream[InputPointer].token_type == Token_Class.WRITE)
                Stats_SetN.Children.Add(Write());
            else if (TokenStream[InputPointer].token_type == Token_Class.READ)
                Stats_SetN.Children.Add(Read());
            else if (TokenStream[InputPointer].token_type == Token_Class.REPEAT)
                Stats_SetN.Children.Add(Repeat());
            else if (TokenStream[InputPointer].token_type == Token_Class.IF)
                Stats_SetN.Children.Add(If_Stat());
            else if (TokenStream[InputPointer].token_type == Token_Class.DATATYPE_FLOAT || TokenStream[InputPointer].token_type == Token_Class.DATATYPE_STRING || TokenStream[InputPointer].token_type == Token_Class.DATATYPE_INT)
                Stats_SetN.Children.Add(Decl_Stat());
            else if (TokenStream[InputPointer].token_type == Token_Class.COMMENT)
                InputPointer++;
            else
                Stats_SetN = null;
            return Stats_SetN;
        }

        Node Write()
        {
            if (InputPointer >= TokenStream.Count)
                return null;
            Node WriteN = new Node("Write Statement");
            WriteN.Children.Add(match(Token_Class.WRITE));
            if (TokenStream[InputPointer].token_type == Token_Class.ENDL)
            {
                WriteN.Children.Add(match(Token_Class.ENDL));
                WriteN.Children.Add(match(Token_Class.SEMICOLON));
            }
            else
            {
                WriteN.Children.Add(Exp());
                WriteN.Children.Add(match(Token_Class.SEMICOLON));
            }
            return WriteN;
        }
        Node Return()
        {
            if (InputPointer >= TokenStream.Count)
                return null;
            Node ReturnN = new Node("Return Statement");
            ReturnN.Children.Add(match(Token_Class.RETURN));
            ReturnN.Children.Add(Exp());
            ReturnN.Children.Add(match(Token_Class.SEMICOLON));
            return ReturnN;
        }
        Node Read()
        {
            if (InputPointer >= TokenStream.Count)
                return null;
            Node ReadN = new Node("Read Statement");
            ReadN.Children.Add(match(Token_Class.READ));
            ReadN.Children.Add(match(Token_Class.IDENTIFIER));
            ReadN.Children.Add(match(Token_Class.SEMICOLON));
            return ReadN;
        }
        Node Repeat()
        {
            if (InputPointer >= TokenStream.Count)
                return null;
            Node RepeatN = new Node("Repeat Statement");
            RepeatN.Children.Add(match(Token_Class.REPEAT));
            RepeatN.Children.Add(Stats_SetD());
            RepeatN.Children.Add(match(Token_Class.UNTIL));
            RepeatN.Children.Add(Cond_Stat());
            return RepeatN;
        }
        Node Assign_Stat()
        {
            if (InputPointer >= TokenStream.Count)
                return null;
            Node Assign_StatN = new Node("Assignment Statement");
            Assign_StatN.Children.Add(match(Token_Class.IDENTIFIER));
            Assign_StatN.Children.Add(match(Token_Class.ASSIGN));
            Assign_StatN.Children.Add(Exp());
            Assign_StatN.Children.Add(match(Token_Class.SEMICOLON));
            return Assign_StatN;
        }
        Node DAssign_Stat()
        {
            if (InputPointer >= TokenStream.Count)
                return null;
            Node Assign_StatN = new Node("Assignment Statement");
            Assign_StatN.Children.Add(match(Token_Class.IDENTIFIER));
            Assign_StatN.Children.Add(match(Token_Class.ASSIGN));
            Assign_StatN.Children.Add(Exp());
            return Assign_StatN;
        }
        Node Exp()
        {
            Node exp = new Node("Expression");
            if (InputPointer >= TokenStream.Count)
                return null;
            if (TokenStream[InputPointer].token_type == Token_Class.STRING)
                exp.Children.Add(match(Token_Class.STRING));
            else if (TokenStream[InputPointer].token_type == Token_Class.NUMBER || TokenStream[InputPointer].token_type == Token_Class.IDENTIFIER)
                if(TokenStream[InputPointer+1].token_type != Token_Class.SEMICOLON)
                    exp.Children.Add(Eq());
                else
                    exp.Children.Add(Term());
            else
                exp.Children.Add(Eq());
            return exp;
        }

        Node Eq()
        {
            if (InputPointer >= TokenStream.Count)
                return null;
            Node eq = new Node("Equation");
            if (TokenStream[InputPointer].token_type == Token_Class.LPARENT)
            {
                eq.Children.Add(match(Token_Class.LPARENT));
                eq.Children.Add(Eq());
                eq.Children.Add(match(Token_Class.RPARENT));
                eq.Children.Add(EqD());
            }
            else
            {
                if (TokenStream[InputPointer].token_type == Token_Class.NUMBER || TokenStream[InputPointer].token_type == Token_Class.IDENTIFIER)
                    eq.Children.Add(Term());
                Node tmpOp = A_Operator();
                eq.Children.Add(tmpOp);
                if (tmpOp != null)
                    eq.Children.Add(Eq());
                else
                { 
                    if (TokenStream[InputPointer].token_type == Token_Class.SEMICOLON)
                        return eq;
                    else
                        eq = null;
                }
            }
            return eq;
        }
        
        Node EqD()
        {
            if (InputPointer >= TokenStream.Count)
                return null;
            Node eqd = new Node("Equations");
            eqd.Children.Add(A_Operator());
            eqd.Children.Add(Eq());
            return eqd;
        }

        Node A_Operator()
        {
            if (InputPointer >= TokenStream.Count)
                return null;
            Node a_operator = new Node("Arithmetic Operator");
            if (TokenStream[InputPointer].token_type == Token_Class.PLUSOP)
                a_operator.Children.Add(match(Token_Class.PLUSOP));
            else if (TokenStream[InputPointer].token_type == Token_Class.MINUSOP)
                a_operator.Children.Add(match(Token_Class.MINUSOP));
            else if (TokenStream[InputPointer].token_type == Token_Class.DIVOP)
                a_operator.Children.Add(match(Token_Class.DIVOP));
            else if (TokenStream[InputPointer].token_type == Token_Class.MULOP)
                a_operator.Children.Add(match(Token_Class.MULOP));
            else
                a_operator = null;
            return a_operator;
        }

        Node Term()
        {
            if (InputPointer >= TokenStream.Count)
                return null;
            Node term = new Node("Term");
            if (TokenStream[InputPointer].token_type == Token_Class.NUMBER)
                term.Children.Add(match(Token_Class.NUMBER));
            else if (TokenStream[InputPointer].token_type == Token_Class.IDENTIFIER &&
                TokenStream[InputPointer + 1].token_type != Token_Class.LPARENT)
                term.Children.Add(match(Token_Class.IDENTIFIER));
            else
                term.Children.Add(Fun_Call());
            return term;
        }

        Node Fun_Call()
        {
            if (InputPointer >= TokenStream.Count)
                return null;
            Node Fun_callN = new Node("Function Call");
            Fun_callN.Children.Add(match(Token_Class.IDENTIFIER));
            Fun_callN.Children.Add(match(Token_Class.LPARENT));
            Fun_callN.Children.Add(match(Token_Class.IDENTIFIER));
            Fun_callN.Children.Add(Id_Arg());
            Fun_callN.Children.Add(match(Token_Class.RPARENT));
            return Fun_callN;
        }

        Node Id_Arg()
        {
            if (InputPointer >= TokenStream.Count)
                return null;
            Node Id_ArgN = new Node("Argument");
            if (TokenStream[InputPointer].token_type == Token_Class.COMMA)
            {
                Id_ArgN.Children.Add(match(Token_Class.COMMA));
                Id_ArgN.Children.Add(match(Token_Class.IDENTIFIER));
                Id_ArgN.Children.Add(Id_Arg());
            }
            else
                Id_ArgN = null;
            return Id_ArgN;
        }


        Node Decl_Stat()
        {
            if (InputPointer >= TokenStream.Count)
                return null;
            Node Decl_StatN = new Node("Declarations Statement");
            if (TokenStream[InputPointer].token_type == Token_Class.DATATYPE_FLOAT)
                Decl_StatN.Children.Add(match(Token_Class.DATATYPE_FLOAT));
            if (TokenStream[InputPointer].token_type == Token_Class.DATATYPE_STRING)
                Decl_StatN.Children.Add(match(Token_Class.DATATYPE_STRING));
            if (TokenStream[InputPointer].token_type == Token_Class.DATATYPE_INT)
                Decl_StatN.Children.Add(match(Token_Class.DATATYPE_INT));
            Decl_StatN.Children.Add(Decl_StatD());
            Decl_StatN.Children.Add(Decl_StatDD());
            Decl_StatN.Children.Add(match(Token_Class.SEMICOLON));
            return Decl_StatN;
        }
        Node Decl_StatDD()
        {
            if (InputPointer >= TokenStream.Count)
                return null;
            Node Decl_StatDDN = new Node("Declaration Statement");
            if (TokenStream[InputPointer].token_type == Token_Class.COMMA)
            { 
                Decl_StatDDN.Children.Add(match(Token_Class.COMMA));
                if (TokenStream[InputPointer].token_type == Token_Class.IDENTIFIER &&
                !(TokenStream[InputPointer + 1].token_type == Token_Class.ASSIGN))
                {
                    Decl_StatDDN.Children.Add(match(Token_Class.IDENTIFIER));
                    Decl_StatDDN.Children.Add(Decl_StatDD());
                }
                else
                {
                    Decl_StatDDN.Children.Add(match(Token_Class.ASSIGN));
                    Decl_StatDDN.Children.Add(Exp());
                    Decl_StatDDN.Children.Add(Decl_StatDD());
                }
            }
            else
                Decl_StatDDN = null;
            return Decl_StatDDN;
        }
        Node Decl_StatD()
        {
            if (InputPointer >= TokenStream.Count)
                return null;
            Node Decl_StatDN = new Node("Declaration Statement");
            if (TokenStream[InputPointer].token_type == Token_Class.IDENTIFIER &&
                !(TokenStream[InputPointer+1].token_type == Token_Class.ASSIGN))
                Decl_StatDN.Children.Add(match(Token_Class.IDENTIFIER));
            else
                Decl_StatDN = DAssign_Stat();
            return Decl_StatDN;
        }
        Node Para()
        {
            Node ParaN = new Node("Parameter");
            if (TokenStream[InputPointer].token_type == Token_Class.DATATYPE_FLOAT)
                ParaN.Children.Add(match(Token_Class.DATATYPE_FLOAT));
            if (TokenStream[InputPointer].token_type == Token_Class.DATATYPE_INT)
                ParaN.Children.Add(match(Token_Class.DATATYPE_INT));
            if (TokenStream[InputPointer].token_type == Token_Class.DATATYPE_STRING)
                ParaN.Children.Add(match(Token_Class.DATATYPE_STRING));
            ParaN.Children.Add(match(Token_Class.IDENTIFIER));
            return ParaN;
        }
        Node ParaD()
        {
            if (InputPointer >= TokenStream.Count)
                return null;
            Node ParaDN = new Node("Parameter");
            if (TokenStream[InputPointer].token_type == Token_Class.COMMA)
            {
                ParaDN.Children.Add(match(Token_Class.COMMA));
                ParaDN.Children.Add(Para());
                ParaDN.Children.Add(ParaD());
            }
            else
                ParaDN = null;
            return ParaDN;
        }

        Node CO()
        {
            if (InputPointer >= TokenStream.Count)
                return null;
            Node node = new Node("Condition Operator");

            if (TokenStream[InputPointer].token_type == Token_Class.LTOP)
                node.Children.Add(match(Token_Class.LTOP));
            else if (TokenStream[InputPointer].token_type == Token_Class.MTOP)
                node.Children.Add(match(Token_Class.MTOP));
            else if (TokenStream[InputPointer].token_type == Token_Class.EQOP)
                node.Children.Add(match(Token_Class.EQOP));
            else if(TokenStream[InputPointer].token_type == Token_Class.NEQOP)
                node.Children.Add(match(Token_Class.NEQOP));
            return node;
        }

        Node Cond()
        {
            if (InputPointer >= TokenStream.Count)
                return null;
            Node node = new Node("Condition");
            node.Children.Add(match(Token_Class.IDENTIFIER));
            node.Children.Add(CO());
            node.Children.Add(Term());
            return node;
        }

        Node Cond_Stat()
        {
            if (InputPointer >= TokenStream.Count)
                return null;
            Node node = new Node("Condition Statement");

            node.Children.Add(Cond());

            if (TokenStream[InputPointer].token_type == Token_Class.AND
                || TokenStream[InputPointer].token_type == Token_Class.OR)
            {
                node.Children.Add(BO());
                node.Children.Add(Cond());
                node.Children.Add(Cond_Stat());
            }
            else
                node.Children.Add(null);

            return node;
        }

        Node If_Stat()
        {
            if (InputPointer >= TokenStream.Count)
                return null;
            Node node = new Node("If Statement");
            node.Children.Add(match(Token_Class.IF));
            node.Children.Add(Cond_Stat());
            node.Children.Add(match(Token_Class.THEN));
            node.Children.Add(Stats_SetD());
            node.Children.Add(ElseIF_StatD());
            node.Children.Add(Else_StatD());
            node.Children.Add(match(Token_Class.END));
            return node;
        }

        Node ElseIF_StatD()
        {
            if (InputPointer >= TokenStream.Count)
                return null;
            Node node = new Node("Elseif Statements");
            Node tmpElseIf = ElseIF_Stat();
            node.Children.Add(tmpElseIf);
            if(!(tmpElseIf == null))
                node.Children.Add(ElseIF_StatD());
            return node;
        }

        Node ElseIF_Stat()
        {
            if (InputPointer >= TokenStream.Count)
                return null;
            Node node = new Node("Elseif Statement");
            if (TokenStream[InputPointer].token_type == Token_Class.ELSEIF)
            {
                node.Children.Add(match(Token_Class.ELSEIF));
                node.Children.Add(Cond());
                node.Children.Add(match(Token_Class.THEN));
                node.Children.Add(Stats_SetD());
            }
            else
                node = null;
            return node;
        }

        Node Else_StatD()
        {
            if (InputPointer >= TokenStream.Count)
                return null;
            Node node = new Node("Else Statements");

            if (TokenStream[InputPointer].token_type == Token_Class.ELSE)
            {
                node.Children.Add(match(Token_Class.ELSE));
                node.Children.Add(Stats_SetD());
            }
            else
                node = null;

            return node;
        }

        Node BO()
        {
            if (InputPointer >= TokenStream.Count)
                return null;
            Node node = new Node("Boolean Operator");
            if (TokenStream[InputPointer].token_type == Token_Class.AND)
                node.Children.Add(match(Token_Class.AND));
            else if (TokenStream[InputPointer].token_type == Token_Class.OR)
                node.Children.Add(match(Token_Class.OR));
            return node;
        }

        public Node match(Token_Class ExpectedToken)
        {
            if (InputPointer < TokenStream.Count)
            {
                if (ExpectedToken == TokenStream[InputPointer].token_type)
                {
                    InputPointer++;
                    Node newNode = new Node(ExpectedToken.ToString());

                    return newNode;

                }
                else
                {
                    Compiler.Syntax_Errors.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + " and " +
                        TokenStream[InputPointer].token_type.ToString() +
                        "  found\r\n");
                    InputPointer++;
                    return null;
                }
            }
            else
            {
                Compiler.Syntax_Errors.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + "\r\n");
                InputPointer++;
                return null;
            }
        }

        public static TreeNode PrintParseTree(Node root)
        {
            TreeNode tree = new TreeNode("Parse Tree");
            TreeNode treeRoot = PrintTree(root);
            if (treeRoot != null)
                tree.Nodes.Add(treeRoot);
            return tree;
        }
        static TreeNode PrintTree(Node root)
        {
            if (root == null || root.Name == null)
                return null;
            TreeNode tree = new TreeNode(root.Name);
            if (root.Children.Count == 0)
                return tree;
            foreach (Node child in root.Children)
            {
                if (child == null)
                    continue;
                tree.Nodes.Add(PrintTree(child));
            }
            return tree;
        }
    }
}
