import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PostsCountres } from '../../data/interface/PostsCountres.interface';

@Injectable({
  providedIn: 'root'
})
export class AnotherService {

  baseApiUrl = 'https://localhost:7247/';
  constructor(private http : HttpClient){}
  
    public getPostsAndCountry() : Observable<PostsCountres>
    {
      return this.http.get<PostsCountres>(`${this.baseApiUrl}get-posts-country`, { withCredentials: true });
    }
  

}
