import ProfileHabit from "./ProfileHabit";

type Profile = {
    username:string,
    id:string,
    dateCreated:string,
    currentHabits:ProfileHabit[],
    currentMonthHabitsCompleted: Record<string,boolean>
}

export default Profile;