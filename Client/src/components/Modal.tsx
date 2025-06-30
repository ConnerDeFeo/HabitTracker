const Modal = (props:{content: React.ReactNode, onClose: ()=>void})=>{
    const { content, onClose } = props;
    return (
        <div 
            className="fixed inset-0 flex items-center justify-center bg-gray-950/70"
            onClick={onClose}
        >
            <div className="bg-white p-6 rounded-lg">
                {content}
            </div>
        </div>
    );
}

export default Modal;