import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TodoService } from './todo.service';
import { Todo } from './todo.model';

@Component({
  selector: 'app-root',
  imports: [FormsModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit {
  private readonly todos = inject(TodoService);

  protected readonly items = signal<Todo[]>([]);
  protected readonly loading = signal(false);
  protected readonly error = signal<string | null>(null);

  // Two-way bound to the "new todo" input.
  protected newTitle = '';

  ngOnInit(): void {
    this.refresh();
  }

  protected refresh(): void {
    this.loading.set(true);
    this.error.set(null);
    this.todos.getAll().subscribe({
      next: items => {
        this.items.set(items);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Could not load todos. Is the API running on http://localhost:5089?');
        this.loading.set(false);
      }
    });
  }

  protected add(): void {
    const title = this.newTitle.trim();
    if (!title) {
      return;
    }

    this.todos.create(title).subscribe({
      next: created => {
        this.items.update(list => [...list, created]);
        this.newTitle = '';
      },
      error: () => this.error.set('Could not add the todo.')
    });
  }

  protected toggle(todo: Todo): void {
    this.todos.update(todo.id, todo.title, !todo.isCompleted).subscribe({
      next: updated => this.items.update(list => list.map(t => (t.id === updated.id ? updated : t))),
      error: () => this.error.set('Could not update the todo.')
    });
  }

  protected remove(todo: Todo): void {
    this.todos.delete(todo.id).subscribe({
      next: () => this.items.update(list => list.filter(t => t.id !== todo.id)),
      error: () => this.error.set('Could not delete the todo.')
    });
  }
}
