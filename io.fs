﻿(*
Copyright (c) 2022 Jun-ichiro Sugisaka

This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
*)
namespace Aqualis
    
    open System
    open System.IO
    open System.Text
    open Aqualis_base

    ///<summary>ファイル入出力</summary>
    type io () =
        
        static member private cat (con:string) (lst:string list) = [0..lst.Length-1] |> List.fold (fun acc i -> acc + (if i=0 then "" else con) + lst.[i]) ""
            
        static member private fileAccess (filename:num0 list) readmode isbinary code =
            let p = p.param
            match p.lang with
              |F ->
                 p.fcache <| fun fp ->
                     let f = 
                       filename
                       |> List.map (fun s -> match s.etype with |Structure("string") -> "A" |It _ -> "I"+p.int_string_format.ToString() |_ -> "")
                       |> io.cat ","
                     let s = 
                       filename
                       |> List.map (fun s -> s.name)
                       |> io.cat ","
                     p.tcache <| A0 <| fun id ->
                         let btname = "byte_tmp"
                         //変数byte_tmpをリストに追加（存在していない場合のみ）
                         match List.exists (fun (_,_,n,_) -> btname=n) (p.vlist) with
                           |false -> p.vlist_add(Structure("integer(1)"),A0,btname,"")
                           |_ -> ()
                         p.codewrite("write("+id+",\"("+f+")\") "+s+"\n")
                         p.getloopvar <| fun (_,counter,_) ->
                             p.codewrite("do "+counter+" = 1, len_trim("+id+")"+"\n")
                             p.codewrite("  if ( "+id+"( "+counter+":"+counter+" ).EQ.\" \" ) "+id+"( "+counter+":"+counter+" ) = \"0\""+"\n")
                             p.codewrite("end do"+"\n")
                         if isbinary then
                             p.codewrite("open("+fp+", file=trim("+id+"), access='stream', form='unformatted')"+"\n")
                         else
                             p.codewrite("open("+fp+", file=trim("+id+"))"+"\n")
                         code(fp)
                         p.codewrite("close("+fp+")"+"\n")
              |C89 |C99 ->
                 p.fcache <| fun fp ->
                     let f = 
                       filename
                       |> List.map (fun s -> match s,s.etype with |Str_e(v),_ -> v |_,It _ -> "%"+p.int_string_format.ToString("00")+"d" |_ -> "")
                       |> io.cat ""
                     let s = 
                       [for s in filename do
                         match s.etype with 
                           |Structure("string") -> ()
                           |_ -> yield s.name ]
                       |> io.cat ","
                     p.tcache <| A0 <| fun id ->
                         let btname = "byte_tmp"
                         //変数byte_tmpをリストに追加（存在していない場合のみ）
                         match List.exists (fun (_,_,n,_) -> btname=n) (p.vlist) with
                           |false -> p.vlist_add(Structure("char"),A0,btname,"")
                           |_ -> ()
                         p.codewrite("sprintf("+id+",\""+f+"\""+(if s="" then "" else ",")+s+");\n")
                         if isbinary then
                             p.codewrite(fp+" = "+"fopen("+id+",\""+(if readmode then "rb" else "wb")+"\");"+"\n")
                         else
                             p.codewrite(fp+" = "+"fopen("+id+",\""+(if readmode then "r" else "w")+"\");"+"\n")
                         code(fp)
                         p.codewrite("fclose("+fp+")"+";\n")
              |T ->
                 p.fcache <| fun fp ->
                     let f = 
                       filename
                       |> List.map (fun s -> match s.etype with |Structure("string") -> "%s" |It _ -> "%"+p.int_string_format.ToString("00")+"d" |_ -> "")
                       |> io.cat ""
                     let s = 
                       filename
                       |> List.map (fun s -> s.name)
                       |> io.cat ","
                     p.tcache <| A0 <| fun id ->
                         let btname = "byte_tmp"
                         //変数byte_tmpをリストに追加（存在していない場合のみ）
                         match List.exists (fun (_,_,n,_) -> btname=n) (p.vlist) with
                           |false -> p.vlist_add(Structure("char"),A0,btname,"")
                           |_ -> ()
                         p.codewrite("sprintf("+id+",\""+f+"\","+s+");\n")
                         if isbinary then
                             p.codewrite(fp+" = "+"fopen("+id+",\""+(if readmode then "rb" else "wb")+"\");"+"\n")
                         else
                             p.codewrite(fp+" = "+"fopen("+id+",\""+(if readmode then "r" else "w")+"\");"+"\n")
                         code(fp)
                         p.codewrite("fclose $"+fp+" "+"$\n")
              |H ->
                 p.fcache <| fun fp ->
                     let f = 
                       filename
                       |> List.map (fun s -> match s.etype with |Structure("string") -> "A" |It _ -> "I"+p.int_string_format.ToString() |_ -> "")
                       |> io.cat ","
                     let s = 
                       filename
                       |> List.map (fun s -> s.name)
                       |> io.cat ","
                     p.codewrite("<span class=\"fio\">file open</span><span class=\"fio\">"+fp+"</span><math><mo>=</mo>"+s+"</math>"+"\n<br/>\n")
                     code(fp)
                     p.codewrite("<span class=\"fio\">file close</span><span class=\"fio\">"+fp+"</span><math></math>\n<br/>\n")
              |NL ->
                ()     
        static member private Write (fp:string) (lst:num0 list) =
            let p = p.param
            match p.lang with
              |F ->
                let tab = var.ip0("tab",2313)
                let double0string_format_F = 
                    let (a,b)=p.double_string_format
                    "E"+a.ToString()+"."+b.ToString()+"e3"
                let format = 
                  lst
                  |> (fun b ->
                      [for n in 0..(b.Length-1) do
                          match b.[n].etype with
                            |It _ -> 
                              yield "I"+p.int_string_format.ToString()
                            |Dt ->
                              yield double0string_format_F
                            |Zt ->
                              yield double0string_format_F
                              yield double0string_format_F 
                            |Structure("string") -> 
                              yield "A"
                            |_ -> ()
                      ])
                  |> (fun b ->
                        [for n in 0..(b.Length-1) do
                            yield b.[n]
                            if n<(b.Length-1) then yield "A1"
                        ])
                  |> io.cat ","
                let code =
                  lst
                  |> (fun b ->
                      [for n in 0..(b.Length-1) do
                          match b.[n].code with 
                            |Int_e(v) -> yield p.ItoS(v)
                            |Dbl_e(v) -> yield p.DtoS(v)
                            |Str_e(v) -> yield v
                            |Var(Zt,_,_) |Code(Zt,_,_) ->
                              yield (b.[n].re.name)
                              yield (b.[n].im.name)
                            |Var(_,v,_) ->
                              yield v
                            |Code(_,v,_) ->
                              yield v
                            |_ -> ()
                      ])
                  |> (fun b ->
                        [for n in 0..(b.Length-1) do
                            yield b.[n]
                            if n<(b.Length-1) then yield tab.name
                        ])
                  |> io.cat ","
                p.codewrite("write("+fp+",\"("+format+")\") "+code+"\n")
              |C89 |C99 ->
                let int0string_format_C =
                  "%"+p.int_string_format.ToString()+"d"
                let double0string_format_C = 
                  let (a,b)=p.double_string_format
                  "%"+a.ToString()+"."+b.ToString()+"e"
                let format = 
                  lst
                  |> (fun b -> 
                      [for n in 0..(b.Length-1) do
                          match b.[n],b.[n].etype with
                            |_,It _ ->
                                yield int0string_format_C
                            |_,Dt ->
                                yield double0string_format_C
                            |_,Zt ->
                                yield double0string_format_C
                                yield double0string_format_C
                            |Str_e(v),_ ->
                                yield v.Replace("\"","\\\"")
                            |_ -> ()
                      ])
                  |> (fun b ->
                        [for n in 0..(b.Length-1) do
                            yield b.[n]
                            if n<(b.Length-1) then yield "\\t"
                        ])
                  |> io.cat ""
                let code =
                  [for b in lst do
                      match b with 
                        |Int_e(v) -> yield p.ItoS(v)
                        |Dbl_e(v) -> yield p.DtoS(v)
                        |Var(Zt,_,_) |Code(Zt,_,_) ->
                          yield b.re.name
                          yield b.im.name
                        |Var(_,n,_) -> yield n
                        |Code(_,n,_) -> yield n
                        |_ -> ()]
                  |> io.cat ","
                p.codewrite("fprintf("+fp+",\""+format+"\\n\""+(if code ="" then "" else ",")+code+");\n")
              |T ->
                let double0string_format_F = 
                    let (a,b)=p.double_string_format
                    "E"+a.ToString()+"."+b.ToString()+"e3"
                let format = 
                  lst
                  |> List.map (fun b -> 
                      match b.etype with
                        |It _ ->"I"+p.int_string_format.ToString()
                        |Dt -> double0string_format_F
                        |Zt -> double0string_format_F+","+double0string_format_F 
                        |Structure("string") -> "A"
                        |_ -> "")
                  |> io.cat ""
                let code =
                  lst
                  |> List.map (fun b ->
                      match b with 
                        |Int_e(v) -> p.ItoS(v)
                        |Dbl_e(v) -> p.DtoS(v)
                        |Str_e(v) -> v
                        |Var(Zt,_,_) |Code(Zt,_,_) -> b.re.name+","+b.im.name
                        |Var(_,n,_) -> n
                        |Code(_,n,_) -> n 
                        |_ -> "")
                  |> io.cat ","
                p.codewrite("write("+fp+",\"("+format+")\") "+code+"\n")
              |H ->
                let double0string_format_F = 
                    let (a,b)=p.double_string_format
                    "E"+a.ToString()+"."+b.ToString()+"e3"
                let format = 
                  lst
                  |> List.map (fun b -> 
                      match b.etype with
                        |It _ ->"I"+p.int_string_format.ToString()
                        |Dt -> double0string_format_F
                        |Zt -> double0string_format_F+","+double0string_format_F 
                        |Structure("string") -> "A"
                        |_ -> "")
                  |> io.cat ""
                let code =
                  lst
                  |> List.map (fun b ->
                      match b with 
                        |Int_e(v) -> p.ItoS(v)
                        |Dbl_e(v) -> p.DtoS(v)
                        |Str_e(v) -> "\""+v+"\""
                        |Var(Zt,_,_) |Code(Zt,_,_) -> b.re.name+","+b.im.name
                        |Var(_,n,_) -> n
                        |Code(_,n,_) -> n 
                        |_ -> "")
                  |> io.cat "<mo>,</mo>"
                p.codewrite("<span class=\"fio\">"+fp+"</span><math><mo>&larr;</mo>"+code+"</math>\n<br/>\n")
              |NL ->
                ()     
                    
        static member private Write_bin (fp:string) (v:num0) =
            let p = p.param
            match p.lang with
              |F ->
                match v.code with 
                  |Int_e(v) ->
                      p.codewrite("write("+fp+") "+p.ItoS(v)+"\n")
                  |Dbl_e(v) ->
                      p.codewrite("write("+fp+") "+p.DtoS(v)+"\n")
                  |Str_e(v) ->
                      p.codewrite("write("+fp+") "+v+"\n")
                  |Var(Zt,_,_) |Code(Zt,_,_) ->
                      p.codewrite("write("+fp+") "+v.re.name+"\n")
                      p.codewrite("write("+fp+") "+v.im.name+"\n")
                  |Var(_,v,_) ->
                      p.codewrite("write("+fp+") "+v+"\n")
                  |Code(_,v,_) ->
                      p.codewrite("write("+fp+") "+v+"\n")
                  |_ -> ()
              |C89 |C99 ->
                match v with 
                  |Int_e _ ->
                    ch.i <| fun tmp ->
                        tmp <== v
                        p.codewrite("fwrite(&"+tmp.name+",sizeof("+tmp.name+"),1,"+fp+");\n")
                  |Dbl_e _ ->
                    ch.i <| fun tmp ->
                        tmp <== v
                        p.codewrite("fwrite(&"+tmp.name+",sizeof("+tmp.name+"),1,"+fp+");\n")
                  |Var(Zt,_,_) ->
                    ch.dd <| fun (tmp_r,tmp_i) ->
                        tmp_r <== v.re
                        tmp_i <== v.im
                        p.codewrite("fwrite(&"+tmp_r.name+",sizeof("+tmp_r.name+"),1,"+fp+");\n")
                        p.codewrite("fwrite(&"+tmp_i.name+",sizeof("+tmp_i.name+"),1,"+fp+");\n")
                  |Var(_,n,_) ->
                    p.codewrite("fwrite(&"+n+",sizeof("+n+"),1,"+fp+");\n")
                  |Code(It _,_,_) ->
                    ch.i <| fun tmp ->
                        tmp <== v
                        p.codewrite("fwrite(&"+tmp.name+",sizeof("+tmp.name+"),1,"+fp+");\n")
                  |Code(Dt,_,_) ->
                    ch.d <| fun tmp ->
                        tmp <== v
                        p.codewrite("fwrite(&"+tmp.name+",sizeof("+tmp.name+"),1,"+fp+");\n")
                  |Code(Zt,_,_) ->
                    ch.ddz <| fun (tmp_r,tmp_i,tmp) ->
                        tmp <== v
                        tmp_r <== tmp.re
                        tmp_i <== tmp.im
                        p.codewrite("fwrite(&"+tmp_r.name+",sizeof("+tmp_r.name+"),1,"+fp+");\n")
                        p.codewrite("fwrite(&"+tmp_i.name+",sizeof("+tmp_i.name+"),1,"+fp+");\n")
                  |_ ->
                    ()
              |T ->
                match v.code with 
                  |Int_e(v) ->
                      p.codewrite("write("+fp+") "+p.ItoS(v)+"\n")
                  |Dbl_e(v) ->
                      p.codewrite("write("+fp+") "+p.DtoS(v)+"\n")
                  |Str_e(v) ->
                      p.codewrite("write("+fp+") "+v+"\n")
                  |Var(Zt,_,_) |Code(Zt,_,_) ->
                      p.codewrite("write("+fp+") "+v.re.name+"\n")
                      p.codewrite("write("+fp+") "+v.im.name+"\n")
                  |Var(_,v,_) ->
                      p.codewrite("write("+fp+") "+v+"\n")
                  |Code(_,v,_) ->
                      p.codewrite("write("+fp+") "+v+"\n")
                  |_ -> ()
              |H ->
                match v.code with 
                  |Int_e(v) ->
                      p.codewrite("<span class=\"fio\">"+fp+"</span><math><mo>&larr;</mo>"+p.ItoS(v)+"</math>\n<br/>\n")
                  |Dbl_e(v) ->
                      p.codewrite("<span class=\"fio\">"+fp+"</span><math><mo>&larr;</mo>"+p.DtoS(v)+"</math>\n<br/>\n")
                  |Str_e(v) ->
                      p.codewrite("<span class=\"fio\">"+fp+"</span><math><mo>&larr;</mo>"+v+"</math>\n<br/>\n")
                  |Var(Zt,_,_) |Code(Zt,_,_) ->
                      p.codewrite("<span class=\"fio\">"+fp+"</span><math><mo>&larr;</mo>"+v.re.name+"</math>\n<br/>\n")
                      p.codewrite("<span class=\"fio\">"+fp+"</span><math><mo>&larr;</mo>"+v.im.name+"</math>\n<br/>\n")
                  |Var(_,v,_) ->
                      p.codewrite("<span class=\"fio\">"+fp+"</span><math><mo>&larr;</mo>"+v+"</math>\n<br/>\n")
                  |Code(_,v,_) ->
                      p.codewrite("<span class=\"fio\">"+fp+"</span><math><mo>&larr;</mo>"+v+"</math>\n<br/>\n")
                  |_ -> ()
              |NL ->
                ()     
                
        static member private Read (fp:string) (iostat:num0) (lst:num0 list) = 
            let p = p.param
            let rec cpxvarlist list (s:num0 list) counter =
                match s with
                  |a::b -> 
                    match a.etype with
                      |Zt -> cpxvarlist <| list@[Zt,counter,a] <| b <| counter+1
                      |t   -> cpxvarlist <| list@[t,0,a] <| b <| counter
                  |[] -> counter,list
            let Nz,varlist = cpxvarlist [] lst 0
    
            match p.lang with
              |F ->
                ch.d01 <| fun tmp ->
                    if Nz>0 then tmp.allocate(2*Nz)
                    let double0string_format_F = 
                      let (a,b)=p.double_string_format
                      "E"+a.ToString()+"."+b.ToString()+"e3"
                    let format = 
                      varlist
                      |> (fun b -> 
                          [for (t,_,_) in b do
                              match t with
                                |It _ ->
                                  yield "I"+p.int_string_format.ToString()
                                |Dt ->
                                  yield double0string_format_F
                                |Zt ->
                                  yield double0string_format_F
                                  yield double0string_format_F
                                |Structure("string") ->
                                  yield "A"
                                |_ -> ()
                          ])
                      |> (fun b ->
                            [for n in 0..(b.Length-1) do
                                yield b.[n]
                                if n<(b.Length-1) then yield "A1"
                            ])
                      |> io.cat ","
                    ch.i1 (varlist.Length-1) <| fun tab ->
                        let code =
                          varlist
                          |> (fun b ->
                              [for (t,m,b) in b do
                                  match t,b with 
                                    |Zt,Var(_) ->
                                      yield tmp.[2*m+1].name
                                      yield tmp.[2*m+2].name
                                    |_,Var(_,n,_) ->
                                      yield n
                                    |_ -> 
                                      Console.WriteLine("ファイル読み込みデータの保存先が変数ではありません")
                                      yield ""
                              ])
                          |> (fun b ->
                                [for n in 0..(b.Length-1) do
                                    yield b.[n]
                                    if n<(b.Length-1) then yield tab.[n+1].name
                                ])
                          |> io.cat ","
                        p.codewrite("read("+fp+",\"("+format+")\",iostat="+iostat.name+") "+code+"\n")
                        for (t,m,b) in varlist do
                            match t with
                              |Zt ->
                                b <== tmp.[2*m+1]+asm.uj*tmp.[2*m+2]
                              |_ ->
                                ()
                    if Nz>0 then tmp.deallocate()
              |C89 ->
                let format = 
                  varlist
                  |> (fun b -> 
                        [for (t,_,_) in b do
                          match t with
                            |It _ ->
                              yield "%d"
                            |Dt -> 
                              yield "%lf"
                            |Zt -> 
                              yield "%lf"
                              yield "%lf"
                            |Structure("string") ->
                              yield "%s"
                            |_ -> ()
                        ])
                  |> io.cat ""
                let code =
                  varlist
                  |> (fun b ->
                        [for (t,_,a) in b do
                          match t,a with 
                            |Zt,Var(_,n,_) ->
                              yield "&"+n+".r"
                              yield "&"+n+".i"
                            |_,Var(_,n,_) ->
                              yield "&"+n
                            |_ ->
                              Console.WriteLine("ファイル読み込みデータの保存先が変数ではありません")
                              yield ""
                        ])
                  |> io.cat ","
                p.codewrite("fscanf("+fp+",\""+format+"\","+code+");\n")
              |C99 ->
                ch.d1 (I <| 2*Nz) <| fun tmp ->
                  let format = 
                    varlist
                    |> (fun b -> 
                          [for (t,_,_) in b do
                            match t with
                              |It _ ->
                                yield "%d"
                              |Dt -> 
                                yield "%lf"
                              |Zt -> 
                                yield "%lf"
                                yield "%lf"
                              |Structure("string") ->
                                yield "%s"
                              |_ -> ()
                          ])
                    |> io.cat ""
                  let code =
                    varlist
                    |> (fun b ->
                          [for (t,m,a) in b do
                            match t,a with 
                              |Zt,Var(_,n,_) ->
                                yield "&"+tmp.[2*m+1].name
                                yield "&"+tmp.[2*m+2].name
                              |_,Var(_,n,_) ->
                                yield "&"+n
                              |_ ->
                                Console.WriteLine("ファイル読み込みデータの保存先が変数ではありません")
                                yield ""
                          ])
                    |> io.cat ","
                  p.codewrite("fscanf("+fp+",\""+format+"\","+code+");\n")
                  for (t,m,b) in varlist do
                      match t with
                        |Zt ->
                          b <== tmp.[2*m+1]+asm.uj*tmp.[2*m+2]
                        |_ ->
                          ()
              |T ->
                let double0string_format_F = 
                  let (a,b)=p.double_string_format
                  "E"+a.ToString()+"."+b.ToString()+"e3"
                let format = 
                  lst
                  |> List.map (fun b -> 
                      match b.etype with
                        |It _ ->"I"+p.int_string_format.ToString()
                        |Dt -> double0string_format_F
                        |Structure("string") -> "A"
                        |_ -> "")
                  |> io.cat ","
                let code =
                  lst
                  |> List.map (fun b ->
                      match b with 
                        |Var(_,n,_) -> n
                        |Code(_,n,_) -> n 
                        |_ -> "")
                  |> io.cat ","
                p.codewrite("read("+fp+",\"("+format+")\",iostat="+iostat.name+") "+code+"\n")
              |H ->
                let double0string_format_F = 
                  let (a,b)=p.double_string_format
                  "E"+a.ToString()+"."+b.ToString()+"e3"
                let format = 
                  lst
                  |> List.map (fun b -> 
                      match b.etype with
                        |It _ ->"I"+p.int_string_format.ToString()
                        |Dt -> double0string_format_F
                        |Structure("string") -> "A"
                        |_ -> "")
                  |> io.cat ","
                let code =
                  lst
                  |> List.map (fun b ->
                      match b with 
                        |Var(_,n,_) -> n
                        |Code(_,n,_) -> n 
                        |_ -> "")
                  |> io.cat "<mo>,</mo>"
                p.codewrite("<math>"+code+"<mo>&larr;</mo></math><span class=\"fio\">"+fp+"</span>\n<br/>\n")
              |NL ->
                ()     
                
        static member private Read_bin (fp:string) (iostat:num0) (v:num0) = 
            let p = p.param
            match p.lang with
              |F ->
                match v with 
                  |Var(Zt,_,_) ->
                    ch.dd <| fun (re,im) ->
                        p.codewrite("read("+fp+",iostat="+iostat.name+") "+re.name+"\n")
                        p.codewrite("read("+fp+",iostat="+iostat.name+") "+im.name+"\n")
                        v <== re+asm.uj*im
                  |Var(_,n,_) ->
                    p.codewrite("read("+fp+",iostat="+iostat.name+") "+n+"\n")
                  |_ -> 
                    Console.WriteLine("ファイル読み込みデータの保存先が変数ではありません")
              |C89|C99 ->
                match v with 
                  |Var(Zt,_,_) ->
                    ch.dd <| fun (re,im) ->
                        p.codewrite("fread(&"+re.name+",sizeof("+re.name+"),1,"+fp+");"+"\n")
                        p.codewrite("fread(&"+im.name+",sizeof("+im.name+"),1,"+fp+");"+"\n")
                        v <== re+asm.uj*im
                  |Var(_,n,_) ->
                    p.codewrite("fread(&"+n+",sizeof("+n+"),1,"+fp+");"+"\n")
                  |_ -> 
                    Console.WriteLine("ファイル読み込みデータの保存先が変数ではありません")
              |T ->
                match v with 
                  |Var(Zt,_,_) ->
                    ch.dd <| fun (re,im) ->
                        p.codewrite("read("+fp+",iostat="+iostat.name+") "+re.name+"\n")
                        p.codewrite("read("+fp+",iostat="+iostat.name+") "+im.name+"\n")
                        v <== re+asm.uj*im
                  |Var(_,n,_) ->
                    p.codewrite("read("+fp+",iostat="+iostat.name+") "+n+"\n")
                  |_ -> 
                    Console.WriteLine("ファイル読み込みデータの保存先が変数ではありません")
              |H ->
                match v with 
                  |Var(_,n,_) ->
                    p.codewrite("<math>"+n+"<mo>&larr;</mo></math><span class=\"fio\">"+fp+"</span>\n<br/>\n")
                  |_ -> 
                    Console.WriteLine("ファイル読み込みデータの保存先が変数ではありません")
              |NL ->
                ()
                
        static member private Read_byte (fp:string) (iostat:num0) (e:num0) = 
            let p = p.param
            p.codewrite("read("+fp+", iostat="+iostat.name+") byte_tmp\n")
            let ee =
                match e with 
                  |Var(It _,n,_) -> n 
                  |_ -> "byte値を整数型以外の変数に格納できません"
            p.codewrite(ee + "=" + "byte_tmp\n")
            
        ///<summary>ファイル出力</summary>
        static member fileOutput (filename:num0 list) code =
            io.fileAccess filename false false <| fun fp ->
                code(io.Write fp)
                
        ///<summary>ファイル出力</summary>
        static member binfileOutput (filename:num0 list) code =
            io.fileAccess filename false true <| fun fp ->
                code(io.Write_bin fp)

        ///<summary>ファイル読み込み</summary>
        static member fileInput (filename:num0 list) code =
            ch.i <| fun iostat ->
                io.fileAccess filename true false <| fun fp ->
                    code(io.Read fp iostat)
                
        ///<summary>バイナリファイルの読み込み</summary>
        static member binfileInput (filename:num0 list) code =
            ch.i <| fun iostat ->
                io.fileAccess filename true true <| fun fp ->
                    code(io.Read_bin fp iostat)
                    
        ///<summary>テキストファイルの行数をカウント</summary>
        static member file_LineCount (counter:num0) (filename:num0 list) varlist =
            ch.i <| fun iostat ->
                io.fileAccess filename true false <| fun fp ->
                    iter.loop <| fun (ext,i) ->
                        io.Read fp iostat varlist
                        br.branch <| fun b ->
                            b.IF (iostat.<0) <| fun () ->
                                counter <== i-1
                                ext()
                                
        ///<summary>ファイルの読み込み</summary>
        static member file_Read (filename:num0 list) varlist code =
            ch.i <| fun iostat ->
                io.fileAccess filename true false <| fun fp ->
                    iter.loop <| fun (ext,i) ->
                        io.Read fp iostat varlist
                        br.branch <| fun b ->
                            b.IF (iostat.<0) <| fun () ->
                                ext()
                            b.EL <| fun () ->
                                code(i)
                                
        ///<summary>配列をファイルに保存</summary>
        static member save_text (f:num2) =
            fun filename ->
                io.fileOutput filename <| fun w -> 
                    iter.array f <| fun (i,j) ->
                        w [i;j;f.[i,j]]
                        
        ///<summary>配列をファイルに保存</summary>
        static member save_text (f:num1) =
            fun filename ->
                io.fileOutput filename <| fun w -> 
                    iter.array f <| fun i -> 
                        w [i;f.[i]]

        ///<summary>1次元データをファイルに保存</summary>
        static member save (f:ax1) =
            fun filename ->
                io.binfileOutput filename <| fun w ->
                    //データフォーマット
                    w _1
                    //データ型
                    match f[1].etype with
                      |Etype.It(4) -> w <| I 1004
                      |Etype.Dt    -> w <| I 2000
                      |Etype.Zt    -> w <| I 3000
                      |_           -> w <| I 0
                    //データ次元
                    w _1
                    //データサイズ
                    w f.size1
                    //データ本体
                    iter.num f.size1 <| fun i ->
                        match f[1].etype with
                          |Zt ->
                            w f[i].re
                            w f[i].im
                          |_ ->
                            w f[i]
                            
        ///<summary>2次元データをファイルに保存</summary>
        static member save (f:ax2) =
            fun filename ->
                io.binfileOutput filename <| fun w ->
                    //データフォーマット
                    w _1
                    //データ型
                    match f[1,1].etype with
                      |Etype.It(4) -> w <| I 1004
                      |Etype.Dt    -> w <| I 2000
                      |Etype.Zt    -> w <| I 3000
                      |_           -> w <| I 0
                    //データ次元
                    w _2
                    //データサイズ
                    w f.size1
                    w f.size2
                    //データ本体
                    iter.num f.size2 <| fun j ->
                        iter.num f.size1 <| fun i ->
                            match f[1,1].etype with
                              |Zt ->
                                w f[i,j].re
                                w f[i,j].im
                              |_ ->
                                w f[i,j]
                            
        ///<summary>3次元データをファイルに保存</summary>
        static member save (f:ax3) =
            fun filename ->
                io.binfileOutput filename <| fun w ->
                    //データフォーマット
                    w _1
                    //データ型
                    match f[1,1,1].etype with
                      |Etype.It(4) -> w <| I 1004
                      |Etype.Dt    -> w <| I 2000
                      |Etype.Zt    -> w <| I 3000
                      |_           -> w <| I 0
                    //データ次元
                    w _3
                    //データサイズ
                    w f.size1
                    w f.size2
                    w f.size3
                    //データ本体
                    iter.num f.size3 <| fun k ->
                        iter.num f.size2 <| fun j ->
                            iter.num f.size1 <| fun i ->
                                match f[1,1,1].etype with
                                  |Zt ->
                                    w f[i,j,k].re
                                    w f[i,j,k].im
                                  |_ ->
                                    w f[i,j,k]

        ///<summary>1次元配列をファイルに保存</summary>
        static member save (f:num1) =
            fun filename ->
                io.save (ax1(f.size1,fun i -> f[i])) filename
                
        ///<summary>2次元配列をファイルに保存</summary>
        static member save (f:num2) =
            fun filename ->
                io.save (ax2(f.size1,f.size2,fun (i,j) -> f[i,j])) filename
                
        ///<summary>3次元配列をファイルに保存</summary>
        static member save (f:num3) =
            fun filename ->
                io.save (ax3(f.size1,f.size2,f.size3,fun (i,j,k) -> f[i,j,k])) filename
                
        ///<summary>1次元データをファイルから読み込み</summary>
        static member load (f:num1) =
            fun filename ->
                let reader (r:num0->unit) (nt:int,t:Etype) =
                    ch.i <| fun n ->
                        //データ型
                        r n
                        br.if2 (n.=nt)
                            <| fun () ->
                                //データ次元
                                r n
                                br.if2 (n.=1)
                                    <| fun () ->
                                        ch.i <| fun n1 ->
                                            //データサイズ
                                            r n1
                                            f.allocate n1
                                            //データ本体
                                            match t with
                                              |Zt ->
                                                iter.num f.size1 <| fun i ->
                                                    ch.dd <| fun (re,im) ->
                                                        r re
                                                        r im
                                                        f[i] <== re + asm.uj*im
                                              |_ ->
                                                iter.num f.size1 <| fun i ->
                                                    r f[i]
                                    <| fun () ->
                                        print.t "Invalid data dimension"
                            <| fun () ->
                                print.s <| filename@[!.": invalid data type"]
                io.binfileInput filename <| fun r ->
                ch.i <| fun n ->
                    //データフォーマット
                    r n
                    br.branch <| fun b ->
                        b.IF (n.=1) <| fun () ->
                            match f[1].etype with
                              |Etype.It(4) ->
                                reader r (1004,f[1].etype)
                              |Etype.Dt    -> 
                                reader r (2000,f[1].etype)
                              |Etype.Zt    -> 
                                reader r (3000,f[1].etype)
                              |_ -> 
                                  print.t "invalid data type"
                                  
        ///<summary>2次元データをファイルから読み込み</summary>
        static member load (f:num2) =
            fun filename ->
                let reader (r:num0->unit) (nt:int,t:Etype) =
                    ch.i <| fun n ->
                        //データ型
                        r n
                        br.if2 (n.=nt)
                            <| fun () ->
                                //データ次元
                                r n
                                br.if2 (n.=2)
                                    <| fun () ->
                                        ch.ii <| fun (n1,n2) ->
                                            //データサイズ
                                            r n1
                                            r n2
                                            f.allocate(n1,n2)
                                            //データ本体
                                            match t with
                                              |Zt ->
                                                iter.num f.size2 <| fun j ->
                                                    iter.num f.size1 <| fun i ->
                                                        ch.dd <| fun (re,im) ->
                                                            r re
                                                            r im
                                                            f[i,j] <== re + asm.uj*im
                                              |_ ->
                                                iter.num f.size2 <| fun j ->
                                                    iter.num f.size1 <| fun i ->
                                                        r f[i,j]
                                    <| fun () ->
                                        print.t "Invalid data dimension"
                            <| fun () ->
                                print.s <| filename@[!.": invalid data type"]
                io.binfileInput filename <| fun r ->
                ch.i <| fun n ->
                    //データフォーマット
                    r n
                    br.branch <| fun b ->
                        b.IF (n.=1) <| fun () ->
                            match f[1,1].etype with
                              |Etype.It(4) ->
                                reader r (1004,f[1,1].etype)
                              |Etype.Dt    -> 
                                reader r (2000,f[1,1].etype)
                              |Etype.Zt    -> 
                                reader r (3000,f[1,1].etype)
                              |_ -> 
                                  print.t "invalid data type"
                                  
        ///<summary>3次元データをファイルから読み込み</summary>
        static member load (f:num3) =
            fun filename ->
                let reader (r:num0->unit) (nt:int,t:Etype) =
                    ch.i <| fun n ->
                        //データ型
                        r n
                        br.if2 (n.=nt)
                            <| fun () ->
                                //データ次元
                                r n
                                br.if2 (n.=3)
                                    <| fun () ->
                                        ch.iii <| fun (n1,n2,n3) ->
                                            //データサイズ
                                            r n1
                                            r n2
                                            r n3
                                            f.allocate(n1,n2,n3)
                                            //データ本体
                                            match t with
                                              |Zt ->
                                                iter.num f.size3 <| fun k ->
                                                    iter.num f.size2 <| fun j ->
                                                        iter.num f.size1 <| fun i ->
                                                            ch.dd <| fun (re,im) ->
                                                                r re
                                                                r im
                                                                f[i,j,k] <== re + asm.uj*im
                                              |_ ->
                                                iter.num f.size3 <| fun k ->
                                                    iter.num f.size2 <| fun j ->
                                                        iter.num f.size1 <| fun i ->
                                                            r f[i,j,k]
                                    <| fun () ->
                                        print.t "Invalid data dimension"
                            <| fun () ->
                                print.s <| filename@[!.": invalid data type"]
                io.binfileInput filename <| fun r ->
                ch.i <| fun n ->
                    //データフォーマット
                    r n
                    br.branch <| fun b ->
                        b.IF (n.=1) <| fun () ->
                            match f[1,1,1].etype with
                              |Etype.It(4) ->
                                reader r (1004,f[1,1,1].etype)
                              |Etype.Dt    -> 
                                reader r (2000,f[1,1,1].etype)
                              |Etype.Zt    -> 
                                reader r (3000,f[1,1,1].etype)
                              |_ -> 
                                  print.t "invalid data type"
                                  
    ///<summary>区切り文字・スペースなしでファイル出力</summary>
    type io2 () =
        
        static member private cat (con:string) (lst:string list) = [0..lst.Length-1] |> List.fold (fun acc i -> acc + (if i=0 then "" else con) + lst.[i]) ""
            
        static member private fileAccess (filename:num0 list) readmode isbinary code =
            let p = p.param
            match p.lang with
              |F ->
                 p.fcache <| fun fp ->
                     let f = 
                       filename
                       |> List.map (fun s -> match s.etype with |Structure("string") -> "A" |It _ -> "I"+p.int_string_format.ToString() |_ -> "")
                       |> io2.cat ","
                     let s = 
                       filename
                       |> List.map (fun s -> s.name)
                       |> io2.cat ","
                     p.tcache <| A0 <| fun id ->
                         let btname = "byte_tmp"
                         //変数byte_tmpをリストに追加（存在していない場合のみ）
                         match List.exists (fun (_,_,n,_) -> btname=n) (p.vlist) with
                           |false -> p.vlist_add(Structure("integer(1)"),A0,btname,"")
                           |_ -> ()
                         p.codewrite("write("+id+",\"("+f+")\") "+s+"\n")
                         p.getloopvar <| fun (_,counter,_) ->
                             p.codewrite("do "+counter+" = 1, len_trim("+id+")"+"\n")
                             p.codewrite("  if ( "+id+"( "+counter+":"+counter+" ).EQ.\" \" ) "+id+"( "+counter+":"+counter+" ) = \"0\""+"\n")
                             p.codewrite("end do"+"\n")
                         if isbinary then
                             p.codewrite("open("+fp+", file=trim("+id+"), access='stream', form='unformatted')"+"\n")
                         else
                             p.codewrite("open("+fp+", file=trim("+id+"))"+"\n")
                         code(fp)
                         p.codewrite("close("+fp+")"+"\n")
              |C89 |C99 ->
                 p.fcache <| fun fp ->
                     let f = 
                       filename
                       |> List.map (fun s -> match s,s.etype with |Str_e(v),_ -> v |_,It _ -> "%"+p.int_string_format.ToString("00")+"d" |_ -> "")
                       |> io2.cat ""
                     let s = 
                       [for s in filename do
                         match s.etype with
                           |Structure("string") -> ()
                           |_ -> yield s.name]
                       |> io2.cat ","
                     p.tcache <| A0 <| fun id ->
                         let btname = "byte_tmp"
                         //変数byte_tmpをリストに追加（存在していない場合のみ）
                         match List.exists (fun (_,_,n,_) -> btname=n) (p.vlist) with
                           |false -> p.vlist_add(Structure("char"),A0,btname,"")
                           |_ -> ()
                         p.codewrite("sprintf("+id+",\""+f+"\""+(if s="" then "" else ",")+s+");\n")
                         if isbinary then
                             p.codewrite(fp+" = "+"fopen("+id+",\""+(if readmode then "rb" else "wb")+"\");"+"\n")
                         else
                             p.codewrite(fp+" = "+"fopen("+id+",\""+(if readmode then "r" else "w")+"\");"+"\n")
                         code(fp)
                         p.codewrite("fclose("+fp+")"+";\n")
              |T ->
                 p.fcache <| fun fp ->
                     let f = 
                       filename
                       |> List.map (fun s -> match s.etype with |Structure("string") -> "%s" |It _ -> "%"+p.int_string_format.ToString("00")+"d" |_ -> "")
                       |> io2.cat ","
                     let s = 
                       filename
                       |> List.map (fun s -> s.name)
                       |> io2.cat ","
                     p.tcache <| A0 <| fun id ->
                         let btname = "byte_tmp"
                         //変数byte_tmpをリストに追加（存在していない場合のみ）
                         match List.exists (fun (_,_,n,_) -> btname=n) (p.vlist) with
                           |false -> p.vlist_add(Structure("char"),A0,btname,"")
                           |_ -> ()
                         p.codewrite("sprintf("+id+",\""+f+"\","+s+");\n")
                         if isbinary then
                             p.codewrite(fp+" = "+"fopen("+id+",\""+(if readmode then "rb" else "wb")+"\");"+"\n")
                         else
                             p.codewrite(fp+" = "+"fopen("+id+",\""+(if readmode then "r" else "w")+"\");"+"\n")
                         code(fp)
                         p.codewrite("fclose $"+fp+" "+"$\n")
              |H ->
                 p.fcache <| fun fp ->
                     let f = 
                       filename
                       |> List.map (fun s -> match s.etype with |Structure("string") -> "A" |It _ -> "I"+p.int_string_format.ToString() |_ -> "")
                       |> io2.cat ","
                     let s = 
                       filename
                       |> List.map (fun s -> s.name)
                       |> io2.cat ","
                     p.codewrite("<span class=\"fio\">file open</span><span class=\"fio\">"+fp+"</span><math><mo>=</mo>"+s+"</math>"+"\n<br/>\n")
                     code(fp)
                     p.codewrite("<span class=\"fio\">file close</span><span class=\"fio\">"+fp+"</span><math></math>\n<br/>\n")
              |NL ->
                ()     
        static member private Write (fp:string) (lst:num0 list) =
            let p = p.param
            match p.lang with
              |F ->
                let format = 
                  lst
                  |> (fun b ->
                      [for n in 0..(b.Length-1) do
                          match b.[n].etype with
                            |It _ -> 
                              yield "I0"
                            |Dt ->
                              yield "F0.3"
                            |Zt ->
                              yield "F0.3"
                              yield "F0.3"
                            |Structure("string") -> 
                              yield "A"
                            |_ -> ()
                      ])
                  |> io2.cat ","
                let code =
                  lst
                  |> (fun b ->
                      [for n in 0..(b.Length-1) do
                          match b.[n].code with 
                            |Int_e(v) -> yield p.ItoS(v)
                            |Dbl_e(v) -> yield p.DtoS(v)
                            |Str_e(v) -> yield v
                            |Var(Zt,_,_) |Code(Zt,_,_) ->
                              yield (b.[n].re.name)
                              yield (b.[n].im.name)
                            |Var(_,v,_) ->
                              yield v
                            |Code(_,v,_) ->
                              yield v
                            |_ -> ()
                      ])
                  |> io2.cat ","
                p.codewrite("write("+fp+",\"("+format+")\") "+code+"\n")
              |C89 |C99 ->
                let int0string_format_C = "%d"
                let double0string_format_C = "%.3f"
                let format = 
                  lst
                  |> (fun b -> 
                      [for n in 0..(b.Length-1) do
                          match b.[n],b.[n].etype with
                            |_,It _ ->
                                yield "%d"
                            |_,Dt ->
                                yield "%.3f"
                            |_,Zt ->
                                yield "%.3f"
                                yield "%.3f"
                            |Str_e(v),_ ->
                                yield v.Replace("\"","\\\"")
                            |_ -> ()
                      ])
                  |> io2.cat ""
                let code =
                  [for b in lst do
                      match b with 
                        |Int_e(v) -> yield p.ItoS(v)
                        |Dbl_e(v) -> yield p.DtoS(v)
                        |Var(Zt,_,_) |Code(Zt,_,_) ->
                          yield b.re.name
                          yield b.im.name
                        |Var(_,n,_) -> yield n
                        |Code(_,n,_) -> yield n 
                        |_ -> ()]
                  |> io2.cat ","
                p.codewrite("fprintf("+fp+",\""+format+"\\n\""+(if code ="" then "" else ",")+code+");\n")
              |T ->
                let code =
                  lst
                  |> List.map (fun b ->
                      match b with 
                        |Int_e(v) -> p.ItoS(v)
                        |Dbl_e(v) -> p.DtoS(v)
                        |Str_e(v) -> "\""+v+"\""
                        |Var(Zt,_,_) |Code(Zt,_,_) -> b.re.name+","+b.im.name
                        |Var(_,n,_) -> n
                        |Code(_,n,_) -> n 
                        |_ -> "")
                  |> io2.cat ","
                p.codewrite("write("+fp+") "+code+"\n")
              |H ->
                let code =
                  lst
                  |> List.map (fun b ->
                      match b with 
                        |Int_e(v) -> p.ItoS(v)
                        |Dbl_e(v) -> p.DtoS(v)
                        |Str_e(v) -> "\""+v+"\""
                        |Var(Zt,_,_) |Code(Zt,_,_) -> b.re.name+","+b.im.name
                        |Var(_,n,_) -> n
                        |Code(_,n,_) -> n 
                        |_ -> "")
                  |> io2.cat "<mo>,</mo>"
                p.codewrite("<span class=\"fio\">"+fp+"</span><math><mo>&larr;</mo>"+code+"</math>\n<br/>\n")
              |NL ->
                ()     
                
        ///<summary>ファイル出力</summary>
        static member fileOutput (filename:num0 list) code =
            io2.fileAccess filename false false <| fun fp ->
                code(io2.Write fp)
                
    ///<summary>ファイル入出力（処理スキップ）</summary>
    type dummy_io () =
        
        static member fileOutput (filename:num0 list) code = ()
        
        static member fileInput (filename:num0 list) code = ()
        
        static member binfileInput (filename:num0 list) code = ()
        
        static member file_LineCount (counter:num0) (filename:num0 list) varlist = ()
        
        static member file_Read (filename:num0 list) varlist code = ()
        
        ///<summary>配列をファイルに保存</summary>
        static member array (f:num2) = fun filename -> ()
        
        ///<summary>配列をファイルに保存</summary>
        static member array (f:num1) = fun filename -> ()
        