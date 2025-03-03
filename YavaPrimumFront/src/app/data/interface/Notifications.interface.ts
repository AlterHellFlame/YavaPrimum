import { DateTime } from "luxon";
import { Tasks } from "./Tasks.interface";
import { User } from "./User.interface";

export interface Notifications
{
    notificationsId: string,
    task: Tasks,
    user: User,
    isReaded: boolean,
    dateTime: DateTime,
    textMessage: string,
    status: string
}
