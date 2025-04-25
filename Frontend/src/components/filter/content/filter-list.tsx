import styles from './filter-list.module.scss';

type props = {
    name: string;
    list: string[];
};

export function FilterList(props: props) {
    const { name, list } = props;

    return (
        <div className={styles.mainContainer}>
            <p className={styles.title}>{name}</p>
            <ul className={styles.list}>
                {list.map((item, index) => {
                    console.log('item:', item);
                    return (
                        <li key={item} className={styles.item}>
                            <input
                                type='checkbox'
                                className={styles.checkbox}
                            />
                            <label>{item}</label>
                        </li>
                    );
                })}
            </ul>
        </div>
    );
}
