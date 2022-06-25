module FSharp.Compiler.SyntaxTreeX.SourceCodeParser

open System

let headerFromFsyacc header =
    let decls = Parser.getDecls("header.fsx",header)
    decls

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

let skip_count = 2

let fromParseTable decls =
    let rec loop acc inps =
        match inps with
        | [] -> failwith $"no found fromParseTable"
        | (XModuleDecl.Let(_,[XBinding(_,[],[],XPat.Named("rules",false,None),_,_)]) as decl)::tail ->
            List.rev acc, decl
        | h::t -> loop (h::acc) t

    let header,letrules = 
        decls
        |> List.skip skip_count
        |> loop []

    let sequential =
        match letrules with
        | XModuleDecl.Let(_,[
            XBinding(_,[],[],_,_,XExpr.Typed(XExpr.ArrayOrListComputed(_,expr),_))
            ]) ->
            expr
        | _ -> failwith $"{letrules}"

    let semans = 
        Parser.getElements sequential
        |> Seq.map(fun expr ->
            match expr with
            | XExpr.Tuple [_;XExpr.Lambda(_,body)] ->
                body
            | _ -> failwith $"never"
        )
        |> Seq.toList
    header,semans
