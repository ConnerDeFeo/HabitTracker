const Modal = (props:{content: React.ReactNode, onClose: ()=>void, display:boolean, className?:string})=>{
    const { content, onClose, display,className } = props;
    return (
        <div 
            className="fixed inset-0 flex items-center justify-center bg-gray-950/70"
            onClick={onClose}
            hidden={!display}
        >
            <div 
                className={`bg-white p-6 rounded-lg ${className}`}
                //This prevent the modal from closing inside the given content
                onClick={(e) => e.stopPropagation()}
            >
                {content}
            </div>
        </div>
    );
}

export default Modal;