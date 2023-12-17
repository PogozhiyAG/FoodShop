let nextId = 0;
let todos = { 
    date :new Date(),
    number: 1
};//[{ id: nextId++, text: 'Todo #1' }];
let date = new Date();
let number = 1;


let listeners = [];

export const todosStore = {
  addTodo() {
    // todos.date = new Date();//[...todos, { id: nextId++, text: 'Todo #' + nextId }]
    // todos.number = todos.number + 1;

    todos = { 
        date :new Date(),
        number: todos.number + 1
    };

    emitChange();
  },
  subscribe(listener) {
    listeners = [...listeners, listener];
    return () => {
      listeners = listeners.filter(l => l !== listener);
    };
  },
  getSnapshot() {
    return todos;
  }
};

function emitChange() {
  for (let listener of listeners) {
    listener();
  }
}
