type Habit ={
    id?: string,
    name:string,
    type: number,
    completed: boolean,
    skipped: boolean,
    value?: number;
    valueUnitType?:string;
    daysActive: string[],
    dateCreated?:string
};

export default Habit;