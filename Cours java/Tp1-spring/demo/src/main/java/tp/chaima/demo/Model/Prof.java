package tp.chaima.demo.model;

import jakarta.persistence.*;
import lombok.*;
import java.util.List;

@Data
@Builder
@NoArgsConstructor
@AllArgsConstructor
@Entity
@Table(name = "prof")
public class Prof {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    private String nom;
    private String matiere;

    @ManyToMany
    @JoinTable(
        name = "prof_user",                         
        joinColumns = @JoinColumn(name = "prof_id"), 
        inverseJoinColumns = @JoinColumn(name = "user_id") 
    )
    private List<User> users;
}