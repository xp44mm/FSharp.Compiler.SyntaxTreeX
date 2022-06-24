module FSharp.Compiler.SyntaxTreeX.SourceCodeParser

open System

let headerFromFsyacc header =
    let decls = 
        Parser.getDecls("header.fsx",header)
    decls

let skip_count = 2

let headerFromParseTable text len =
    let decls = 
        Parser.getDecls("parsetable.fs",text)
    decls 
    |> List.skip skip_count
    |> List.take len

let semansFromFsyacc mappers =
    let decls = Parser.getDecls("semans.fsx",mappers)
    let bodies =
        decls
        |> List.map(fun decl ->
            match decl with
            | XModuleDecl.Expr(XExpr.Lambda(_,body)) ->
                body
            | _ -> failwith $"{decl}"
        )
    bodies
        
let semansFromParseTable text =
    let decls = 
        Parser.getDecls("parsetable.fs",text)

    let decl =
        decls
        |> List.find(fun decl ->
            match decl with
            | XModuleDecl.Let(_,[XBinding(_,[],[],XPat.Named("rules",false,None),_,_)]) ->
                true
            | _ -> false
        )

    let sequential =
        match decl with
        | XModuleDecl.Let(_,[XBinding(_,[],[],_,_,XExpr.Typed(XExpr.ArrayOrListComputed(_,expr),_))]) ->
            expr
        | _ -> failwith $"{decl}"

    let bodies = 
        Parser.getElements sequential
        |> Seq.map(fun expr ->
            match expr with
            | XExpr.Tuple [_;XExpr.Lambda(_,body)] ->
                body
            | _ -> failwith $"never"
        )
        |> Seq.toList
    bodies

