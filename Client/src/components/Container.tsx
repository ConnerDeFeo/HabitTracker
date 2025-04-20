import { ReactNode } from "react";

const Container = (props: { content: ReactNode})=>{
    const { content } = props;
    return(
        <div className="flex flex-col justify-end min-h-[60vh] mb-[20vh]">
            {content}
        </div>
    );
} 

export default Container;