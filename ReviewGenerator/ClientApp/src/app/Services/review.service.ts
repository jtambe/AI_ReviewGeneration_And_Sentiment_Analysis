import { Inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, retry, tap } from 'rxjs/operators';
import { Review } from '../models/Review';


@Injectable({
  providedIn: 'root'
})


export class ReviewService {

  constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string) { }

  public review: Review;

  getReview() {

    //for some reason baseUrl is not working
    return this.http.get<Review>('https://localhost:44386/api/review/');

  }
}
