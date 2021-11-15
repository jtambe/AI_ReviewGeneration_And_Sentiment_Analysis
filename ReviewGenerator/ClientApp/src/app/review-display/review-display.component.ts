import { Component, OnInit } from '@angular/core';
import { ReviewService } from '../Services/review.service';
import { Review } from '../models/Review';

@Component({
  selector: 'app-review-display',
  templateUrl: './review-display.component.html',
  styleUrls: ['./review-display.component.css']
})
export class ReviewDisplayComponent implements OnInit {

  constructor(private reviewService: ReviewService) { }

  ngOnInit(): void {
  }


  public review: Review;
  public generateReview() {

    this.reviewService.getReview()
      .subscribe(data => this.review = {
        reviewText: (data as any).reviewText,
        overall: (data as any).overall,
    });
    

  }


}
