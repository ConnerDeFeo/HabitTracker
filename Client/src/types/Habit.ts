type Habit ={
    id?: string,
    name:string,
    type: number,
    completed: boolean,
    value?: number;
    valueUnitType?:string;
};

export default Habit;