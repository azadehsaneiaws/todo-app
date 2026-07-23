import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { Todo } from './todo.model';

/**
 * Thin wrapper around the TODO REST API. Requests use the relative `/api`
 * path and are forwarded to the backend by the dev-server proxy
 * (see proxy.conf.json), so the browser never hits a cross-origin call.
 */
@Injectable({ providedIn: 'root' })
export class TodoService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = '/api/todos';

  getAll(): Observable<Todo[]> {
    return this.http.get<Todo[]>(this.baseUrl);
  }

  create(title: string): Observable<Todo> {
    return this.http.post<Todo>(this.baseUrl, { title });
  }

  update(id: string, title: string, isCompleted: boolean): Observable<Todo> {
    return this.http.put<Todo>(`${this.baseUrl}/${id}`, { title, isCompleted });
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
