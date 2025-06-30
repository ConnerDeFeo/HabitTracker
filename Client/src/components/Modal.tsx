const Modal = (props:{content: React.ReactNode})=>{
    return (
        <div className="fixed inset-0 flex items-center justify-center bg-gray-950/70">
            <div className="bg-white p-6 rounded-lg">
                {props.content}
            </div>
        </div>
    );
}

export default Modal;