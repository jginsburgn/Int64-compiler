/*
Authors:
- Gad Levy A01017986
- Jonathan Ginsburg A01021617
- Pablo de la Mora A01020365
*/

using System;
using System.Collections.Generic;

namespace Int64 {

    class SemanticAnalyzer {

        static string MAIN_LEXEME = "main";

        //-----------------------------------------------------------

        public List<FunctionSym> functions = new List<FunctionSym>();
        public List<VariableSym> globalVars = new List<VariableSym>();

        //-----------------------------------------------------------

        private FirstPassVisitor firstPassVisitor;
        private NProgram ast;

        //-----------------------------------------------------------

        public SemanticAnalyzer(NProgram newAst) {
            ast = newAst;
            AddStandardFunctionsToSymTable();
        }

        public void AddFunction(FunctionSym newFunction) {
            foreach (FunctionSym function in functions) {
                if (function.anchorToken.Lexeme == newFunction.anchorToken.Lexeme) {
                    throw new SemanticError("Name collision", newFunction.anchorToken);
                }
            }
            functions.Add(newFunction);
        }

        public void AddGlobalVariable(VariableSym newVariable) {
            foreach (VariableSym variable in globalVars) {
                if (variable.anchorToken.Lexeme == newVariable.anchorToken.Lexeme) {
                    throw new SemanticError("Name collision", newVariable.anchorToken);
                }
            }
            globalVars.Add(newVariable);
        }

        private void AddStandardFunctionsToSymTable() {
            // Add standard functions to symbol table.
            AddFunction(newStandardFunction("printi", "i"));
            AddFunction(newStandardFunction("printc", "c"));
            AddFunction(newStandardFunction("prints", "s"));
            AddFunction(newStandardFunction("println"));
            AddFunction(newStandardFunction("readi"));
            AddFunction(newStandardFunction("reads"));
            AddFunction(newStandardFunction("new", "n"));
            AddFunction(newStandardFunction("size", "h"));
            AddFunction(newStandardFunction("add", "h", "x"));
            AddFunction(newStandardFunction("get", "h", "x"));
            AddFunction(newStandardFunction("set", "h", "i", "x"));
        }

        public void Analyze() {
            this.firstPassVisitor = new FirstPassVisitor(this);
            firstPassVisitor.Visit(ast);
            ensureMainExists();
        }

        private void ensureMainExists() {
            foreach (FunctionSym function in functions) {
                if (function.anchorToken.Lexeme == MAIN_LEXEME && function.GetArity() == 0) return;
            }
            throw new SemanticError("No main function declared");
        }

        private FunctionSym newStandardFunction(string symbol, params string[] parameters) {
            FunctionSym retVal = new FunctionSym(new Token(symbol, TokenCategory.IDENTIFIER, -1, -1), FunctionSymKind.STANDARD);
            for (int i = 0; i < parameters.Length; i++) {
               retVal.AddLocalVariable(new VariableSym(new Token(parameters[i], TokenCategory.IDENTIFIER, -1, -1), VariableSymKind.PARAMETER)); 
            }
            return retVal;
        }
    }
}
