type Habit ={
    id: string,
    name:string,
    type: string,
    completed: boolean,
    value?: number;
    valueUnitType?:string;
};

export default Habit;