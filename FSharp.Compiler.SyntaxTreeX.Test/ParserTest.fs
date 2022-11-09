namespace FSharp.Compiler.SyntaxTreeX

open Xunit
open Xunit.Abstractions

open System
open System.IO
open System.Text

open FSharp.Literals
open FSharp.xUnit

open FslexFsyacc.Fslex
open FslexFsyacc.Fsyacc


type ParserTest(output: ITestOutputHelper) =
    let show res =
        res |> Render.stringify |> output.WriteLine

    [<Fact>]
    member _.``valid ParseTable``() =
        let fsyaccParseTableFile =
            let text = File.ReadAllText(Dir.fsyacc, Encoding.UTF8)
            let rawFsyacc = RawFsyaccFile.parse text
            let fsyacc = FlatFsyaccFile.fromRaw rawFsyacc
            fsyacc.toFsyaccParseTableFile()

        let headerFromFsyacc =
            let decls =
                FSharp.Compiler.SyntaxTreeX.Parser.getDecls("header.fsx",fsyaccParseTableFile.header)
            decls

        let semansFsyacc =
            let mappers = fsyaccParseTableFile.generateMappers()
            FSharp.Compiler.SyntaxTreeX.SourceCodeParser.semansFromMappers mappers

        let header,semans =
            let text = File.ReadAllText(Dir.parseTable, Encoding.UTF8)
            FSharp.Compiler.SyntaxTreeX.SourceCodeParser.getHeaderSemansFromFSharp 2 text

        Should.equal headerFromFsyacc header
        Should.equal semansFsyacc semans

    [<Fact>]
    member _.``valid DFA``() =
        let dfafile =
            let text = File.ReadAllText(Dir.fslex, Encoding.UTF8)
            let fslexFile = FslexFile.parse text
            let dfafile = fslexFile.toFslexDFAFile()
            dfafile

        let headerFromFslex =
            let decls =
                FSharp.Compiler.SyntaxTreeX.Parser.getDecls("header.fsx",dfafile.header)
            decls

        let mappers =
            dfafile.rules
            |> List.map(fun(_,_,semantic) ->
                LexSemanticGenerator.decorateSemantic semantic
                )
            |> String.concat Environment.NewLine
        //output.WriteLine(mappers)

        let semansFslex =
            FSharp.Compiler.SyntaxTreeX.SourceCodeParser.semansFromMappers mappers

        let header,semans =
            let text = File.ReadAllText(Dir.dfa, Encoding.UTF8)
            FSharp.Compiler.SyntaxTreeX.SourceCodeParser.getHeaderSemansFromFSharp 1 text

        Should.equal headerFromFslex header
        Should.equal semansFslex semans

