import { ReactNode } from "react";

/*Formats content to stay at the bottom of the screen preferably, then rise as vh is lifted*/
const Container = (props: { content: ReactNode})=>{
    const { content} = props;
    return(
        <div className='flex flex-col justify-end min-h-[60vh] mb-[10vh]'>
            {content}
        </div>
    );
} 

export default Container;