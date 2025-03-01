import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http'; // Используйте правильный импорт
import { PostsCountries } from '../../data/interface/PostsCountries.interface';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class OptionalDataService {

  baseApiUrl = 'https://localhost:7247/';
  constructor(private http : HttpClient) {}
  
  public getPostsAndCountry(): Observable<PostsCountries> {
    return this.http.get<PostsCountries>(`${this.baseApiUrl}get-posts-countries`, { withCredentials: true });
  }
}
