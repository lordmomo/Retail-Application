import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest, HttpResponse } from "@angular/common/http";
import { catchError, Observable, tap, throwError } from "rxjs";
import { Injectable } from "@angular/core";
import { Router } from "@angular/router";

@Injectable({
    providedIn:'root'
})
export class UniversalAppInterceptor implements HttpInterceptor{
    
    constructor(private router : Router){

    }

    
    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

        const token :string | null = sessionStorage.getItem('token')

        if( token != null
        ){
            req = req.clone({headers : req.headers.set('Authorization', 'Bearer '+token)})
        }

        return next.handle(req).pipe(
            tap(event => {
                if (event instanceof HttpResponse) {
                    console.log('Response received');
                }
            }),
            catchError(error => {
                console.error('HTTP Error:', error);

                if (error.status === 401) {
                    this.handleTokenExpiration();
                }
                return throwError(error);
            })
        );
    }

    private handleTokenExpiration(): void {
        sessionStorage.removeItem('token');
        this.router.navigate(['/login']); 
    }

}