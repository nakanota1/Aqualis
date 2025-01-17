﻿(*
Copyright (c) 2022 Jun-ichiro Sugisaka

This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
*)
namespace Aqualis
    
    open Aqualis_base
    
    module fft2 = 
        
        type fftw_plan2(name) =
            static member sname = "fftw_plan"
            new(name,c) =
                str.reg(fftw_plan2.sname,name,c)
                fftw_plan2(name)
            member __.name = name
            
        let fftshift2(x:num2) =
            br.if2 (x.size2%2 .= 0)
                <| fun () ->
                    iter.num x.size1 <| fun i ->
                        fft1.fftshift_even(x[i,()])
                <| fun () ->
                    iter.num x.size1 <| fun i ->
                        fft1.fftshift_odd(x[i,()])
            br.if2 (x.size1%2 .= 0)
                <| fun () ->
                    iter.num x.size2 <| fun i ->
                        fft1.fftshift_even(x[(),i])
                <| fun () ->
                    iter.num x.size2 <| fun i ->
                        fft1.fftshift_odd(x[(),i])
                        
        let ifftshift2(x:num2) =
            br.if2 (x.size2%2 .= 0)
                <| fun () ->
                    iter.num x.size1 <| fun i ->
                        fft1.ifftshift_even(x[i,()])
                <| fun () ->
                    iter.num x.size1 <| fun i ->
                        fft1.ifftshift_odd(x[i,()])
            br.if2 (x.size1%2 .= 0)
                <| fun () ->
                    iter.num x.size2 <| fun i ->
                        fft1.ifftshift_even(x[(),i])
                <| fun () ->
                    iter.num x.size2 <| fun i ->
                        fft1.ifftshift_odd(x[(),i])

        let private fft2(planname:string,data1:num2,data2:num2,fftdir:int) =
            p.param.option_("-lfftw3")
            p.param.option_("-I/usr/include")
            ch.iiii <| fun (nx,ny,nx2,ny2) -> 
                nx <== data1.size1
                ny <== data1.size2
                nx2 <== data1.size1./_2
                ny2 <== data1.size2./_2
                match p.param.lang with
                  |F ->
                    p.param.include_("'fftw3.f'")
                    let plan = var.i1(planname, 8)
                    if fftdir=1 then
                        p.param.codewrite("call dfftw_plan_dft_2d(" + plan.name + ", " + nx.name + ", " + ny.name + ", " + data1.name + ", " + data2.name + ", FFTW_FORWARD, FFTW_ESTIMATE )")
                        fftshift2(data1)
                        !"FFTを実行"
                        p.param.codewrite("call dfftw_execute(" + plan.name + ")")
                        fftshift2(data2)
                        p.param.codewrite("call dfftw_destroy_plan(" + plan.name + ")")
                    else
                        p.param.codewrite("call dfftw_plan_dft_2d(" + plan.name + ", " + nx.name + ", " + ny.name + ", " + data1.name + ", " + data2.name + ", FFTW_BACKWARD, FFTW_ESTIMATE )")
                        ifftshift2(data1)
                        !"FFTを実行"
                        p.param.codewrite("call dfftw_execute(" + plan.name + ")")
                        ifftshift2(data2)
                        p.param.codewrite("call dfftw_destroy_plan(" + plan.name + ")")
                  |C89|C99 ->
                    p.param.include_("\"fftw3.h\"")
                    let plan = fftw_plan2(planname)
                    if fftdir=1 then
                        p.param.codewrite(plan.name + " = fftw_plan_dft_2d(" + nx.name + ", "+ ny.name + ", " + data1.name + ", " + data2.name + ", FFTW_FORWARD, FFTW_ESTIMATE);")
                        fftshift2(data1)
                        !"FFTを実行"
                        p.param.codewrite("call dfftw_execute(" + plan.name + ")")
                        fftshift2(data2)
                        p.param.codewrite("call dfftw_destroy_plan(" + plan.name + ")")
                    else
                        p.param.codewrite(plan.name + " = fftw_plan_dft_2d(" + nx.name + ", "+ ny.name + ", " + data1.name + ", " + data2.name + ", FFTW_BACKWARD, FFTW_ESTIMATE);")
                        ifftshift2(data1)
                        !"FFTを実行"
                        p.param.codewrite("dfftw_execute(" + plan.name + ")")
                        ifftshift2(data2)
                        p.param.codewrite("dfftw_destroy_plan(" + plan.name + ")")
                  |T ->
                    p.param.codewrite(data2.name + " = \\mathcal{F}\left[" + data1.name + "\right]")
                  |H ->
                    p.param.codewrite(data2.name + " = <mi mathvariant=\"script\">F</mi><mfenced open=\"[\" close=\"]\">" + data1.name + "</mfenced>")
                  |NL ->
                    ()
                if fftdir=1 then
                    !"規格化"
                    iter.num nx <| fun i ->
                        iter.num ny <| fun j ->
                            data2.[i,j]<==data2.[i,j]/(nx*ny)
                        
        let fft(planname:string,data1:num2,data2:num2) =
                fft2(planname,data1,data2,1)
                
        let ifft(planname:string,data1:num2,data2:num2) =
                fft2(planname,data1,data2,-1)
                